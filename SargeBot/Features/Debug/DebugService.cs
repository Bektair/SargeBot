using SC2APIProtocol;

namespace SargeBot.Features.Debug;

public class DebugService
{
    public DebugCommand DrawText(string text)
    {
        uint size = 16;
        var x = 0.1f;
        var y = 0.1f;

        return new()
        {
            Draw = new() {Text = {new DebugText {Text = text, Size = size, VirtualPos = new() {X = x, Y = y}}}}
        };
    }
}