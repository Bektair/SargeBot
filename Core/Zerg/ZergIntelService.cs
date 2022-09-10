using SC2APIProtocol;

namespace Core.Zerg;

public class ZergIntelService : IntelService, IZergIntelService
{
    public ZergIntelService(IEnumerable<IDataService> dataServices) : base(dataServices)
    {
    }

    public override Race Race => Race.Zerg;

    public IList<IUnit> GetHatchesWithoutInject()
    {
        return GetUnits(UnitType.ZERG_HATCHERY)
            .Where(x => !x.BuffIds.Contains((uint)Buff.QueenSpawnLarvaTimer))
            .ToList();
    }
}

public interface IZergIntelService
{
    public IList<IUnit> GetHatchesWithoutInject();
}