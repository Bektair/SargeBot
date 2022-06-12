using SargeBot.Features.Intel;
using SC2APIProtocol;
using SC2ClientApi;
using SC2ClientApi.Constants;
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

            if (!unit.UnitType.Is(UnitTypes.ZERG_OVERLORD))
                continue;

            var command = new ActionRawUnitCommand();
            command.UnitTags.Add(unit.Tag);
            command.AbilityId = (int) Abilities.MOVE;
            command.TargetWorldSpacePos = _intelService.EnemyColonies.First().Point;

            return new() {ActionRaw = new() {UnitCommand = command}};
        }

        return new() {ActionRaw = new()};
    }
    public Action ZerglingAttack(ResponseObservation observation)
    {
        foreach (var unit in observation.Observation.RawData.Units)
        {
            if (unit.Alliance != Alliance.Self)
                continue;

            if (!unit.UnitType.Is(UnitTypes.ZERG_ZERGLING))
                continue;

            var command = new ActionRawUnitCommand();
            command.UnitTags.Add(unit.Tag);
            command.AbilityId = (int) Abilities.ATTACK;
            command.TargetWorldSpacePos = _intelService.EnemyColonies.First().Point;

            return new() {ActionRaw = new() {UnitCommand = command}};
        }

        return new() {ActionRaw = new()};
    }
}