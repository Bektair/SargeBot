using Core.Intel;
using Core.Macro;
using Core.SharedModels;
using SC2APIProtocol;

namespace Core.Zerg;

public class ZergMacroService : MacroService
{
    private readonly IZergBuildingPlacement _buildingPlacement;

    public ZergMacroService(IMessageService messageService, IEnumerable<IIntelService> intelServices, IZergBuildingPlacement buildingPlacement) : base(
        messageService, intelServices)
    {
        _buildingPlacement = buildingPlacement;
    }

    public override Race Race => Race.Zerg;

    public override void Train(UnitType unitType, Point2D? rallyPoint = null)
    {
        if (!ZergDataHelpers.Producers.TryGetValue(unitType, out var producers))
            throw new NotImplementedException($"Producer for {unitType} not found");

        var producer = producers.First();

        var producerUnits = IntelService.GetUnits(producer.Type);

        MessageService.Action(producer.Ability, producerUnits.Select(x => x.Tag));
    }


    public List<Point2D> _BaseLocations = new List<Point2D>();

    int count = 0;

    public override void Build(UnitType unit, int allocatedWorkerCount)
    {
        uint unitType = (uint)unit;
        var builders = IntelService.GetUnits(UnitType.ZERG_DRONE)
        .Where(x => !x.Orders.Any(order => order.AbilityId.IsWorkerBuildAbillity()))
           .Select(x => x.Tag)
           .Take(allocatedWorkerCount);
        ZergDataHelpers.Producers.TryGetValue(unit, out var producers);
        var producer = producers.First();

        if (unitType.IsVespeneCollectingBuilding())
        {
            ulong? target = _buildingPlacement.FindPlacementGas();
            if (target != null)
            {
                MessageService.Action(producer.Ability, builders, (ulong)target);
            }

        }
        else if (unitType.Is(UnitType.ZERG_HATCHERY))
        {
            if (IntelService.playerMinerals >= 300)
            {
                count++;
                var newPoint = IntelService.BaseLocations[count % 16];
                if (newPoint != null) { 
                    MessageService.Action(producer.Ability, builders, newPoint);
                    _BaseLocations.Add(newPoint);
                }
            }
        }
    }
}