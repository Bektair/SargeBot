using SC2APIProtocol;

namespace Core.Zerg;

public class ZergMacroService : MacroService
{
    private readonly IZergIntelService _zergIntelService;

    public ZergMacroService(IMessageService messageService, IEnumerable<IIntelService> intelServices, IZergIntelService zergIntelService) : base(
        messageService, intelServices)
    {
        _zergIntelService = zergIntelService;
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
}