using SC2APIProtocol;

namespace SargeBot.Features.Debug;

public class DebugService
{
    private readonly SC2Client _sc2Client;

    public DebugService(SC2Client sc2Client)
    {
        _sc2Client = sc2Client;
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

        await _sc2Client.SendRequest(request);
    }
}