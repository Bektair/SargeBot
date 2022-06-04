using SC2APIProtocol;

namespace SargeBot.Features.Debug;

public class DebugService
{
    public static DebugCommand DrawText(string text, uint size = 16, float x = 0.1f, float y = 0.1f) =>
        new() {Draw = new() {Text = {new DebugText {Text = text, Size = size, VirtualPos = new() {X = x, Y = y}}}}};

    public static DebugCommand DrawLine(Point start, Point end, Color color) =>
        new() {Draw = new() {Lines = {new DebugLine {Color = color, Line = new() {P0 = start, P1 = end}}}}};

    public static DebugCommand DrawSphere(Point point, float radius = 2, Color? color = null) =>
        new() {Draw = new() {Spheres = {new DebugSphere {Color = color, P = point, R = radius}}}};

    public static DebugCommand DrawBox(Point min, Point max, Color color) =>
        new() {Draw = new() {Boxes = {new DebugBox {Color = color, Max = max, Min = min}}}};
}