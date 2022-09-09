using SC2APIProtocol;

namespace Core.Intel;

public class IntelColony : IColony
{
    // public List<IUnit> Minerals { get; set; } = new();
    // public List<Unit> VespeneGas { get; set; } = new();
    public Point2D Point { get; set; } = new();
    public bool IsStartingLocation { get; set; }
}