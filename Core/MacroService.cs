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

        var builders = IntelService.GetStructures(producer.Type)
            .Where(x => x.BuildProgress == 1)
            .Where(x => !x.Orders.Any())
            .Select(x => x.Tag);

        MessageService.Action(producer.Ability, builders);
    }

    /// <summary>
    ///     Should be replaced with production queue
    /// </summary>
    public virtual void Build(UnitType unitType, int allocatedWorkerCount)
    {
        if (!TerranDataHelpers.Producers.TryGetValue(unitType, out var producers) &&
            !ProtossDataHelpers.Producers.TryGetValue(unitType, out producers))
            throw new NotImplementedException($"Producer for {unitType} not found");

        var producer = producers.First();

        var builders = IntelService.GetWorkers()
            .Select(x => x.Tag)
            .Take(allocatedWorkerCount);

        var location = BuildingPlacement.Random(IntelService.Colonies.First().Point);
        
        MessageService.Action(producer.Ability, builders, location);
    }
}

public interface IMacroService
{
    Race Race { get; }
    void Train(UnitType unitType);
    void Build(UnitType unitType, int allocatedWorkerCount = 1);
}