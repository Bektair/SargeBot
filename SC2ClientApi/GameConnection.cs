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
    private const int TIMEOUT_LONG = 10000; //ms
    private readonly ResponseHandler _responseHandler;
    private readonly CancellationToken _token = CancellationToken.None;
    private ClientWebSocket? _socket;
    private Status _status;

    public GameConnection()
    {
        _responseHandler = new();
    }

    private string Version { get; set; } = string.Empty;

    public Status Status
    {
        get => _status;
        private set
        {
            if (_status == value) return;
            Log.Info($"GameConnection Status changed: {_status} -> {value}");
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
                Log.Info($"Connection attempt {connectionAttempt}/{maxAttempts} failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Info($"Connection attempt {connectionAttempt}/{maxAttempts} failed: {ex.Message}");
            }

            connectionAttempt++;
            await Task.Delay(TIMEOUT, _token);
        } while (_socket.State != WebSocketState.Open && connectionAttempt <= maxAttempts);

        if (_socket.State != WebSocketState.Open)
            return false;

        Log.Info("Connection success. Starting receive forever task");
        Task.Factory.StartNew(ReceiveForever, TaskCreationOptions.LongRunning);
        await Task.Delay(TIMEOUT);

        var pingResponse = await SendAndReceiveAsync(ClientConstants.RequestPing);
        if (pingResponse == null)
        {
            Log.Info("Ping failed");
            return false;
        }

        Version = pingResponse.Ping.GameVersion;
        Log.Info($"Ping success. Version {Version}");
        return pingResponse.Ping.HasGameVersion;
    }

    public async Task<Response?> SendAndReceiveAsync(Request req)
    {
        Response? response = null;

        if (_socket.State != WebSocketState.Open)
        {
            Log.Info($"Can't send request due to socket state {_socket.State}");
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
        var shouldWaitLonger = req.RequestCase is Request.RequestOneofCase.Step or Request.RequestOneofCase.JoinGame or Request.RequestOneofCase.CreateGame;
        if (!handlerResolve.Wait(shouldWaitLonger ? TIMEOUT_LONG : TIMEOUT)) Log.Info($"Request timed out \n{req}");
        _responseHandler.DeregisterHandler(req.RequestCase);

        return response;
    }

    public async Task SendAsync(Request req) => await _socket.SendAsync
        (new(req.ToByteArray()), WebSocketMessageType.Binary, true, _token);

    [Obsolete("Remove when handlers are okay")]
    private async Task<Response> ReceiveAsync()
    {
        var buffer = new ArraySegment<byte>(new byte[READ_BUFFER]);
        WebSocketReceiveResult result;
        using var ms = new MemoryStream();

        do
        {
            result = await _socket.ReceiveAsync(buffer, _token);
            ms.Write(buffer.Array, buffer.Offset, result.Count);
        } while (!result.EndOfMessage);

        ms.Seek(0, SeekOrigin.Begin);

        var response = Response.Parser.ParseFrom(ms);
        Status = response.Status;

        return response;
    }

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
            _responseHandler.Handle(response.ResponseCase, response);
        }
    }

    public async Task Disconnect()
    {
        await _socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, _token);
        _socket.Dispose();
    }
}