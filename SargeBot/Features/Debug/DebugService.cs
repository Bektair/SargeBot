using SC2APIProtocol;

namespace SargeBot.Features.Debug;

public class DebugService
{
    private readonly SC2ClientApi.GameClient _gameClient;

    public DebugService(SC2ClientApi.GameClient gameClient)
    {
        _gameClient = gameClient;
    }

    public async Task DrawText(string text)
    {
        uint size = 16;
        var x = 0.1f;
        var y = 0.1f;

        var command = new DebugCommand
        {
            Draw = new() {Text = {new DebugText {Text = text, Size = size, VirtualPos = new() {X = x, Y = y}}}}
        };

        await SendDebugRequest(command);
    }

    private async Task SendDebugRequest(DebugCommand debugCommand)
    {
        var request = new Request
        {
            Debug = new()
            {
                Debug =
                {
                    debugCommand
                }
            }
        };

        await _gameClient.SendRequest(request);
    }
}