using SC2APIProtocol;

namespace SargeBot.Features.Macro.Building.Zerg;

public class ZergBuildingPlacement
{
    private readonly IIntelService _intelService;

    public ZergBuildingPlacement(IIntelService intelService)
    {
        _intelService = intelService;
    }

    public Point2D FindPlacement()
    {
        var mainBase = _intelService.Colonies.First(x => x.IsStartingLocation);

        //  consider copy paste Sharkys circle math

        // temporary random placement
        var r = new Random();
        var x = mainBase.Point.X + r.Next(-10, 10);
        var y = mainBase.Point.Y + r.Next(-10, 10);

        return new Point2D { X = x, Y = y };
    }
}