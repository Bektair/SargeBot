using Core.SharedModels;
using SC2APIProtocol;

namespace Core.Extensions;

public static class PointExtensions
{
    public static Point2D ConvertTo2D(this Point point)
    {
        return new Point2D { X = point.X, Y = point.Y };
    }

    public static T GetClosest<T>(this IPosition point, IEnumerable<T> enumerable) where T : IPosition
    {
        return GetClosest(point.Point, enumerable);
    }

    public static T GetClosest<T>(this Point2D point, IEnumerable<T> enumerable) where T : IPosition
    {
        IPosition result = null;
        var minValue = double.MaxValue;
        double value;
        foreach (var position in enumerable)
        {
            value = FastDistance(point, position.Point);
            if (value < minValue)
            {
                minValue = value;
                result = position;
            }
        }

        return (T)result;
    }
    
    public static double Distance(this Point2D point, float x, float y)
    {
        return Math.Sqrt(FastDistance(point, x, y));
    }

    public static double Distance(this Point2D point, Point2D otherPoint)
    {
        return Math.Sqrt(FastDistance(point, otherPoint));
    }

    public static double FastDistance(this Point2D point, Point2D otherPoint)
    {
        return point.FastDistance(otherPoint.X, otherPoint.Y);
    }

    public static double FastDistance(this Point2D point, float x, float y)
    {
        var deltaX = point.X - x;
        var deltaY = point.Y - y;
        return deltaX * deltaX + deltaY * deltaY;
    }

    /// <summary>
    ///     Stolen from https://stackoverflow.com/questions/55550198/how-to-get-a-point-between-two-points
    /// </summary>
    public static Point2D GetPointBetweenPoints(this Point2D start, Point2D end, double distanceDelta)
    {
        var distance = Distance(start, end) + distanceDelta;

        var angle = Math.Atan2(end.Y - start.Y, start.X - end.X);

        return new Point2D
        {
            X = (float)(end.X + distance * Math.Cos(angle)),
            Y = (float)(end.Y - distance * Math.Sin(angle))
        };
    }
}