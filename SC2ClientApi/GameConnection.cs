using System.Net.WebSockets;
using Google.Protobuf;
using SC2APIProtocol;
using SC2ClientApi.Constants;

namespace SC2ClientApi;

internal class GameConnection
{
    private const int READ_BUFFER = 1024;
    private const int MAX_CONNECTION_ATTEMPTS = 25;
    private const int TIMEOUT = 2000; //ms
    private const int TIMEOUT_LONG = 5000; //ms
    private readonly ResponseHandler _responseHandler;
    private readonly CancellationToken _token = CancellationToken.None;
    private ClientWebSocket? _socket;
    private Status _status;

    public GameConnection()
    {
        _responseHandler = new();
    }

    private string GameVersion { get; set; } = string.Empty;

    public Status Status
    {
        get => _status;
        private set
        {
            if (_status == value) return;
            Log.Info($"ConnectionStatus changed {_status} -> {value}");
            _status = value;
        }
    }

    public async Task<bool> Connect(string address, int port, int maxAttempts = MAX_CONNECTION_ATTEMPTS)
    {
        var uri = new Uri($"ws://{address}:{port}/sc2api");
        Log.Info($"Connecting to {uri}");

        var connectionAttempt = 1;
        do
        {
            try
            {
                _socket = new();
                await _socket.ConnectAsync(uri, _token);
            }
            catch (AggregateException ex)
            {
                // handle AggEx differently?
                Log.Info($"Connection to {uri} attempt {connectionAttempt}/{maxAttempts} failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Info($"Connection to {uri} attempt {connectionAttempt}/{maxAttempts} failed: {ex.Message}");
            }

            connectionAttempt++;
            await Task.Delay(TIMEOUT, _token);
        } while (_socket.State != WebSocketState.Open && connectionAttempt <= maxAttempts);

        if (_socket.State != WebSocketState.Open)
            return false;

        Log.Success($"Connected to {uri}. Start receiving responses from client");
        Task.Factory.StartNew(ReceiveForever, TaskCreationOptions.LongRunning);
        await Task.Delay(TIMEOUT);

        var pingResponse = await SendAndReceiveAsync(ClientConstants.RequestPing);
        if (pingResponse == null)
        {
            Log.Error($"Ping to {uri} failed");
            return false;
        }

        Log.Info($"Pinged {uri} - [GameVersion={pingResponse.Ping.GameVersion}] [DataVersion={pingResponse.Ping.DataVersion}] [DataBuild={pingResponse.Ping.DataBuild}] [BaseBuild={pingResponse.Ping.BaseBuild}]");
        GameVersion = pingResponse.Ping.GameVersion;
        return pingResponse.Ping.HasGameVersion;
    }

    public async Task<Response?> SendAndReceiveAsync(Request req)
    {
        Response? response = null;

        if (_socket.State != WebSocketState.Open)
        {
            Log.Warning($"Can't send request due to socket state {_socket.State}");
            return response;
        }

        var handlerResolve = new Task(() => { });
        var handler = new Action<Response>(r =>
        {
            response = r;
            handlerResolve.RunSynchronously();
        });

        _responseHandler.RegisterHandler(req.RequestCase, handler);
        await SendAsync(req);
        //TODO: find out why join requests never get any responses
        if (req.RequestCase is not Request.RequestOneofCase.JoinGame)
        {
            var shouldWaitLonger = req.RequestCase is Request.RequestOneofCase.Step or Request.RequestOneofCase.JoinGame or Request.RequestOneofCase.CreateGame;
            if (!handlerResolve.Wait(shouldWaitLonger ? TIMEOUT_LONG : TIMEOUT)) Log.Error($"Request timed out \n{req}");
        }

        _responseHandler.DeregisterHandler(req.RequestCase, handler);

        return response;
    }

    private async Task SendAsync(Request req) => await _socket.SendAsync
        (new(req.ToByteArray()), WebSocketMessageType.Binary, true, _token);

    private async Task ReceiveForever()
    {
        var buffer = new ArraySegment<byte>(new byte[READ_BUFFER]);
        while (true)
        {
            WebSocketReceiveResult result;
            using var ms = new MemoryStream();
            do
            {
                result = await _socket.ReceiveAsync(buffer, CancellationToken.None);
                ms.Write(buffer.Array, buffer.Offset, result.Count);
            } while (!result.EndOfMessage);

            var response = Response.Parser.ParseFrom(ms.GetBuffer(), 0, (int) ms.Position);
            Status = response.Status;
            _responseHandler.Handle((Request.RequestOneofCase) response.ResponseCase, response);
        }
    }

    public async Task Disconnect()
    {
        await _socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, _token);
        _socket.Dispose();
    }
}