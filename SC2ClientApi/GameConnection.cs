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

    public Status Status { get; private set; }

    public async Task<bool> Connect(Uri uri, int maxAttempts = MAX_CONNECTION_ATTEMPTS)
    {
        var failCount = 0;
        do
        {
            try
            {
                _socket = new();
                await _socket.ConnectAsync(uri, CancellationToken.None);
            }
            catch (AggregateException)
            {
                await Task.Delay(TIMEOUT);
                failCount++;
            }
            catch (Exception)
            {
                break;
            }
        } while (_socket.State != WebSocketState.Open && failCount < maxAttempts);

        if (_socket.State != WebSocketState.Open)
            return false;

        var pingResponse = await SendAndReceiveAsync(ClientConstants.RequestPing);
        return pingResponse.Ping.HasGameVersion;
    }

    public async Task<Response> SendAndReceiveAsync(Request req)
    {
        await SendAsync(req);
        return await ReceiveAsync();
    }

    private async Task<Response> ReceiveAsync()
    {
        var buffer = new ArraySegment<byte>(new byte[READ_BUFFER]);
        var result = await _socket.ReceiveAsync(buffer, CancellationToken.None);
        using var ms = new MemoryStream();

        do
        {
            ms.Write(buffer.Array, buffer.Offset, result.Count);
        } while (!result.EndOfMessage);

        ms.Seek(0, SeekOrigin.Begin);

        var response = Response.Parser.ParseFrom(ms);
        Status = response.Status;
        
        return response;
    }

    public async Task SendAsync(Request req) => await _socket.SendAsync(new(req.ToByteArray()), WebSocketMessageType.Binary, true, CancellationToken.None);

    public async Task Disconnect()
    {
        await _socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        _socket.Dispose();
    }
}