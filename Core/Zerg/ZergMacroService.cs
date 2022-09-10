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

    public override void Train(UnitType unitType)
    {
        if (!ZergDataHelpers.Producers.TryGetValue(unitType, out var producers))
            throw new NotImplementedException($"Producer for {unitType} not found");

        var producer = producers.First();

        var producerUnits = IntelService.GetUnits(producer.Type);

        MessageService.Action(producer.Ability, producerUnits.Select(x => x.Tag));
    }

  public override void Build(UnitType unitType, int allocatedWorkerCount)
  {
    var builders = IntelService.GetUnits(UnitType.ZERG_DRONE)
       .Select(x => x.Tag)
       .Take(allocatedWorkerCount);
    ZergDataHelpers.Producers.TryGetValue(unitType, out var producers);
    var producer = producers.First();

    if (unitType == UnitType.ZERG_EXTRACTOR) 
    {
      ulong? target = _buildingPlacement.FindPlacementGas();
      if (target != null) {
        MessageService.Action(producer.Ability, builders, (ulong)target);
      }

    }
    else if(unitType == UnitType.ZERG_SPAWNINGPOOL)
    {
      
      

    }
  }


}