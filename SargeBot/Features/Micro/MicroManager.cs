using SargeBot.Features.Intel;
using SC2APIProtocol;
using SC2ClientApi;
using Action = SC2APIProtocol.Action;

namespace SargeBot.Features.Micro;

public class MicroManager
{
    private readonly IntelService _intelService;

    public MicroManager(IntelService intelService)
    {
        _intelService = intelService;
    }

    public Action OverlordScout(ResponseObservation observation)
    {
        foreach (var unit in observation.Observation.RawData.Units)
        {
            if (unit.Alliance != Alliance.Self)
                continue;

            if (!unit.UnitType.Is(UnitType.ZERG_OVERLORD))
                continue;

            var command = new ActionRawUnitCommand();
            command.UnitTags.Add(unit.Tag);
            command.AbilityId = (int) Ability.move_Move;
            command.TargetWorldSpacePos = _intelService.EnemyColonies.First().Point;

            return new() {ActionRaw = new() {UnitCommand = command}};
        }

        return new() {ActionRaw = new()};
    }

    public Action AttackWithAll(ResponseObservation observation, UnitType unitType)
    {
        var units = observation.Observation.RawData.Units
            .Where(u => u.Alliance == Alliance.Self)
            .Where(u => u.UnitType.Is(unitType));
        foreach (var unit in units)
        {
            var command = new ActionRawUnitCommand();
            command.UnitTags.Add(unit.Tag);
            command.AbilityId = (int) Ability.ATTACK;
            command.TargetWorldSpacePos = _intelService.EnemyColonies.First().Point;

            return new() {ActionRaw = new() {UnitCommand = command}};
        }

        return new() {ActionRaw = new()};
    }
}