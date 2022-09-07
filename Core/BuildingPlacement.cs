using SC2APIProtocol;

namespace Core;

public class BuildingPlacement
{
    public static Point2D Random(Point2D startingPoint)
    {
        //  consider copy paste Sharkys circle math

        // temporary random placement
        var r = new Random();
        var x = startingPoint.X + r.Next(-10, 10);
        var y = startingPoint.Y + r.Next(-10, 10);

        return new Point2D { X = x, Y = y };
    }
}