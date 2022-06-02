using System.Net.WebSockets;
using Google.Protobuf;
using SC2APIProtocol;

namespace SC2ClientApi;

internal class GameConnection
{
    private const int READ_BUFFER = 1024 * 1024;
    private const int MAX_CONNECTION_ATTEMPTS = 25;
    private const int TIMEOUT = 550; //ms

    private ClientWebSocket? _socket;
    private CancellationToken token = new CancellationTokenSource().Token;

    public Status Status { get; private set; }
    public Response PingResponse { get; private set; }

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
                continue;
            }
        } while (_socket.State != WebSocketState.Open && failCount < maxAttempts);

        if (_socket.State != WebSocketState.Open)
            return false;

        PingResponse = await SendAndReceiveAsync(ClientConstants.RequestPing);
        return PingResponse.Ping.HasGameVersion;
    }

    public async Task<Response> SendAndReceiveAsync(Request req)
    {

        await SendAsync(req);

        Response debugMe = await ReceiveAsync();
        return debugMe;
    }
    public async Task SendAsync(Request req) => await _socket.SendAsync
     (new(req.ToByteArray()), WebSocketMessageType.Binary, true, token);

    private async Task<Response> ReceiveAsync()
    {
        var buffer = new ArraySegment<byte>(new byte[READ_BUFFER]);
        WebSocketReceiveResult result = null;
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

    public async Task Disconnect()
    {
        await _socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, token);
        _socket.Dispose();
    }
}