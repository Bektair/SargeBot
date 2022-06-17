using SargeBot.Features.Macro.Building.Zerg;
using SC2APIProtocol;
using SC2ClientApi;
using Action = SC2APIProtocol.Action;

namespace SargeBot.Features.Macro;

/// <summary>
///     Builds
///     Trains
///     Upgrades
/// </summary>
public class MacroManager
{
    private readonly ZergBuildingPlacement _zergBuildingPlacement;

    public MacroManager(ZergBuildingPlacement zergBuildingPlacement)
    {
        _zergBuildingPlacement = zergBuildingPlacement;
    }

    public Action BuildSpawningPool(ResponseObservation observation)
    {
        foreach (var unit in observation.Observation.RawData.Units)
        {
            if (unit.Alliance != Alliance.Self)
                continue;

            if (!unit.UnitType.Is(UnitType.ZERG_DRONE))
                continue;

            var command = new ActionRawUnitCommand();
            command.UnitTags.Add(unit.Tag);
            command.AbilityId = (int) Ability.BUILD_SPAWNINGPOOL;
            command.TargetWorldSpacePos = _zergBuildingPlacement.FindPlacement();

            return new() {ActionRaw = new() {UnitCommand = command}};
        }

        return new() {ActionRaw = new()};
    }

    public static Action MorphLarva(ResponseObservation observation, Ability ability)
    {
        foreach (var unit in observation.Observation.RawData.Units)
        {
            if (unit.Alliance != Alliance.Self)
                continue;

            if (!unit.UnitType.Is(UnitType.ZERG_LARVA))
                continue;

            var command = new ActionRawUnitCommand();
            command.UnitTags.Add(unit.Tag);
            command.AbilityId = (int) ability;

            return new() {ActionRaw = new() {UnitCommand = command}};
        }

        return new() {ActionRaw = new()};
    }
}