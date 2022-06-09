using SC2APIProtocol;

namespace SargeBot.Features.Intel;

public class IntelColony
{
    public Point2D Point { get; set; } = new();
    public bool IsStartingLocation { get; set; }
    public List<Unit> Minerals { get; set; } = new();
    public List<Unit> VespeneGas { get; set; } = new();
}