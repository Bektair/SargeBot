using SC2APIProtocol;

namespace Core;

public static class DebugRequest
{
    public static DebugCommand DrawText(string text, uint size = 16, float x = 0.1f, float y = 0.1f, Color? color = null, bool worldPosition = false)
    {
        var command = new DebugCommand { Draw = new DebugDraw { Text = { new DebugText { Color = color, Text = text, Size = size } } } };

        if (worldPosition)
            command.Draw.Text.First().WorldPos = new Point { X = x, Y = y };
        else
            command.Draw.Text.First().VirtualPos = new Point { X = x, Y = y };

        return command;
    }

    public static DebugCommand DrawLine(Point start, Point end, Color? color = null)
    {
        return new DebugCommand { Draw = new DebugDraw { Lines = { new DebugLine { Color = color, Line = new Line { P0 = start, P1 = end } } } } };
    }

    public static DebugCommand DrawSphere(Point point, float radius = 2, Color? color = null)
    {
        return new DebugCommand { Draw = new DebugDraw { Spheres = { new DebugSphere { Color = color, P = point, R = radius } } } };
    }

    public static DebugCommand DrawBox(Point min, Point max, Color? color = null)
    {
        return new DebugCommand { Draw = new DebugDraw { Boxes = { new DebugBox { Color = color, Max = max, Min = min } } } };
    }
}