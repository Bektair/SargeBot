using System.Net.WebSockets;
using Google.Protobuf;
using SC2APIProtocol;
using SC2ClientApi.Constants;

namespace SC2ClientApi;

internal class GameConnection
{
    private const int READ_BUFFER = 1024 * 1024;
    private const int MAX_CONNECTION_ATTEMPTS = 25;
    private const int TIMEOUT = 550; //ms
    private readonly ResponseHandler _responseHandler;
    private readonly CancellationToken token = new CancellationTokenSource().Token;
    private ClientWebSocket? _socket;

    public GameConnection()
    {
        _responseHandler = new();
    }

    public Status Status { get; private set; }

    public async Task<bool> Connect(Uri uri, int maxAttempts = MAX_CONNECTION_ATTEMPTS)
    {
        var failCount = 0;
        do
        {
            try
            {
                _socket = new();
                await _socket.ConnectAsync(uri, token);
            }
            catch (AggregateException)
            {
                await Task.Delay(TIMEOUT);
                failCount++;
            }
            catch (Exception)
            {
                await Task.Delay(TIMEOUT);
                failCount++;
            }
        } while (_socket.State != WebSocketState.Open && failCount < maxAttempts);

        if (_socket.State != WebSocketState.Open)
            return false;

        Task.Run(ReceiveForever);

        var pingResponse = await SendAndReceiveAsync(ClientConstants.RequestPing);
        return pingResponse.Ping.HasGameVersion;
    }

    public async Task<Response> SendAndReceiveAsync(Request req)
    {
        Response response = null;

        var marker = new Task(() => { });
        var handler = new Action<Response>(r =>
        {
            response = r;
            marker.RunSynchronously();
        });

        _responseHandler.RegisterHandler(req.RequestCase, handler);
        await SendAsync(req);
        marker.Wait(TIMEOUT);
        _responseHandler.DeregisterHandler(req.RequestCase);

        return response;
    }

    public async Task SendAsync(Request req) => await _socket.SendAsync
        (new(req.ToByteArray()), WebSocketMessageType.Binary, true, token);

    [Obsolete("Remove when handlers are okay")]
    private async Task<Response> ReceiveAsync()
    {
        var buffer = new ArraySegment<byte>(new byte[READ_BUFFER]);
        WebSocketReceiveResult result;
        using var ms = new MemoryStream();

        do
        {
            result = await _socket.ReceiveAsync(buffer, token);
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
        while (_socket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result;
            using (var ms = new MemoryStream())
            {
                do
                {
                    result = await _socket.ReceiveAsync(buffer, CancellationToken.None);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                var msg = Response.Parser.ParseFrom(ms.GetBuffer(), 0, (int) ms.Position);
                Status = msg.Status;
                _responseHandler.Handle(msg.ResponseCase, msg);
            }
        }
    }

    public async Task Disconnect()
    {
        await _socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, token);
        _socket.Dispose();
    }
}