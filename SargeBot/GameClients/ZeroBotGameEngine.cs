using SC2APIProtocol;
using SC2ClientApi;
using Action = SC2APIProtocol.Action;

namespace SargeBot.GameClients;

public class ZeroBotGameEngine : IGameEngine
{
    private Point2D? _enemyStartingLocation;
    private bool _isAttacking;
    private int _previousDroneCount;

    public void OnStart(ResponseObservation firstObservation, ResponseData responseData, ResponseGameInfo gameInfo)
    {
        Log.Info($"OnStart {firstObservation.Observation.GameLoop}");

        _enemyStartingLocation = gameInfo.StartRaw.StartLocations.Last();
    }

    public (List<Action>, List<DebugCommand>) OnFrame(ResponseObservation observation)
    {
        if (observation.Observation.GameLoop <= 5 || observation.Observation.GameLoop % 100 == 0)
            Log.Info($"OnFrame {observation.Observation.GameLoop}");

        var actions = new List<Action>();
        var debugCommands = new List<DebugCommand>();

        actions.Add(MorphLarva(observation, Ability.TRAIN_DRONE));
        var droneCount = observation.Observation.RawData.Units.Count(u => u.UnitType.Is(UnitType.ZERG_DRONE));
        if (droneCount > _previousDroneCount) Log.Info($"Drone count {droneCount}");

        if (droneCount >= 14)
        {
            actions.Add(AttackWithAll(observation, UnitType.ZERG_DRONE, _enemyStartingLocation));
            if (!_isAttacking) Log.Success($"Attacking {_enemyStartingLocation}");

            _isAttacking = true;
        }

        _previousDroneCount = droneCount;

        return (actions, debugCommands);
    }

    public void OnEnd(ResponseObservation? observation)
    {
        if (observation == null) return;
        Log.Info($"OnEnd {observation.Observation.GameLoop} {observation.PlayerResult.ToString()}");
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

    public Action AttackWithAll(ResponseObservation observation, UnitType unitType, Point2D? target)
    {
        var units = observation.Observation.RawData.Units
            .Where(u => u.Alliance == Alliance.Self)
            .Where(u => u.UnitType.Is(unitType));

        foreach (var unit in units)
        {
            var command = new ActionRawUnitCommand();
            command.UnitTags.Add(unit.Tag);
            command.AbilityId = (int) Ability.ATTACK;
            command.TargetWorldSpacePos = target;

            return new() {ActionRaw = new() {UnitCommand = command}};
        }

        return new() {ActionRaw = new()};
    }
}