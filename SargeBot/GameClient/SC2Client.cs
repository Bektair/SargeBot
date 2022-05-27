using System.Net.WebSockets;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using SargeBot.Options;
using SC2APIProtocol;

namespace SargeBot;


public class SC2Client
{
    private ClientWebSocket? ClientSocket;
    private readonly string Address;
    private readonly int Port;

    private CancellationToken Token = new CancellationTokenSource().Token;


    public SC2Client(IOptions<GameConnectionOptions> options)
    {
        
        Address = options.Value.Address;
        Port = options.Value.Port;
    }

    public async Task Connect()
    {
        ClientSocket = new ClientWebSocket();
        ClientSocket.Options.KeepAliveInterval = TimeSpan.FromDays(30);
        string adr = string.Format("ws://{0}:{1}/sc2api", Address, Port);
        Uri uri = new (adr);

        await ClientSocket.ConnectAsync(uri, Token);
    }

    public async Task Ping()
    {
        Request request = new Request();
        request.Ping = new RequestPing();
        Response response = await SendRequest(request);
    }

    public async Task<Response> SendRequest(Request request)
    {
        await WriteMessage(request);
        return await ReadMessage();
    }

    public async Task Quit()
    {
        Request quit = new Request();
        quit.Quit = new RequestQuit();
        await WriteMessage(quit);
    }

    private async Task WriteMessage(Request request)
    {
        byte[] sendBuf = new byte[1024 * 1024];
        CodedOutputStream outStream = new CodedOutputStream(sendBuf);
        request.WriteTo(outStream);
        await ClientSocket.SendAsync(new ArraySegment<byte>(sendBuf, 0, (int)outStream.Position), WebSocketMessageType.Binary, true, Token);
    }

    private async Task<Response> ReadMessage()
    {
        byte[] receiveBuf = new byte[1024 * 1024];
        bool finished = false;
        int curPos = 0;
        while (!finished)
        {
            int left = receiveBuf.Length - curPos;
            if (left < 0)
            {
                // No space left in the array, enlarge the array by doubling its size.
                byte[] temp = new byte[receiveBuf.Length * 2];
                Array.Copy(receiveBuf, temp, receiveBuf.Length);
                receiveBuf = temp;
                left = receiveBuf.Length - curPos;
            }
            WebSocketReceiveResult result = await ClientSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuf, curPos, left), Token);
            if (result.MessageType != WebSocketMessageType.Binary)
            {
                throw new Exception("Expected Binary message type.");
            }

            curPos += result.Count;
            finished = result.EndOfMessage;
        }

        Response response = Response.Parser.ParseFrom(new System.IO.MemoryStream(receiveBuf, 0, curPos));

        return response;
    }
}
