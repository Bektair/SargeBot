using Core.Intel;
using Core.Terran;
using SC2APIProtocol;

namespace Core;

public abstract class MacroService : IMacroService
{
    public readonly IIntelService IntelService;
    public readonly IMessageService MessageService;

    public MacroService(IMessageService messageService, IEnumerable<IIntelService> intelServices)
    {
        MessageService = messageService;
        IntelService = intelServices.First(x => x.Race == Race);
    }

    public abstract Race Race { get; }

    /// <summary>
    ///     Should be replaced with production queue
    /// </summary>
    public virtual void Train(UnitType unitType)
    {
        if (!TerranDataHelpers.Producers.TryGetValue(unitType, out var producers) &&
            !ProtossDataHelpers.Producers.TryGetValue(unitType, out producers))
            throw new NotImplementedException($"Producer for {unitType} not found");

        var producer = producers.First();

        var structures = IntelService.GetStructures(producer.Type).Select(x => x.Tag);

        MessageService.Action(producer.Ability, structures);
    }
}

public interface IMacroService
{
    Race Race { get; }
    void Train(UnitType unitType);
}