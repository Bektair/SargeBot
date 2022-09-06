using Core.Intel;
using SC2APIProtocol;

namespace Core.Zerg;

public class ZergMacroService : MacroService
{
    private readonly ILarvaService _larvaService;

    public ZergMacroService(IMessageService messageService, IEnumerable<IIntelService> intelServices, ILarvaService larvaService) : base(
        messageService, intelServices)
    {
        _larvaService = larvaService;
    }

    public override Race Race => Race.Zerg;

    public override void Train(UnitType unitType)
    {
        if (!ZergDataHelpers.Producers.TryGetValue(unitType, out var producers))
            throw new NotImplementedException($"Producer for {unitType} not found");

        var producer = producers.First();

        var producerUnits = producer.Type == UnitType.ZERG_LARVA
            ? _larvaService.GetLarva()
            : IntelService.GetStructures(producer.Type);

        MessageService.Action(producer.Ability, producerUnits.Select(x => x.Tag));
    }
}