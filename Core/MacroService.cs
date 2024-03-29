﻿using Core.Terran;
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
    public virtual void Train(UnitType unitType, Point2D? rallyPoint = null)
    {
        if (!TerranDataHelpers.Producers.TryGetValue(unitType, out var producers) &&
            !ProtossDataHelpers.Producers.TryGetValue(unitType, out producers))
            throw new NotImplementedException($"Producer for {unitType} not found");

        var producer = producers.First();

        var eligibleBuilders = IntelService.GetUnits(producer.Type)
            .Where(x => x.BuildProgress > .99)
            .Where(x => !x.Orders.Any())
            .Select(x => x.Tag)
            .ToList();

        MessageService.Action(producer.Ability, eligibleBuilders);

        if (rallyPoint != null) MessageService.Action(Ability.Rally_Building, eligibleBuilders, rallyPoint);
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

        var builders = IntelService.GetUnits(producer.Type)
            .Select(x => x.Tag)
            .Take(allocatedWorkerCount);

        var location = BuildingPlacement.Random(IntelService.Colonies.First().Point);

        MessageService.Action(producer.Ability, builders, location);
    }
}

public interface IMacroService
{
    Race Race { get; }
    void Train(UnitType unitType, Point2D? rallyPoint = null);
    void Build(UnitType unitType, int allocatedWorkerCount = 1);
}