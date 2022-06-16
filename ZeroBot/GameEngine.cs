using SC2APIProtocol;
using SC2ClientApi;
using SC2ClientApi.Constants;
using Action = SC2APIProtocol.Action;

namespace ZeroBot.GameClients;

public class GameEngine : IGameEngine
{
    private Point2D? _enemyStartingLocation;
    private bool _isAttacking;
    private int _previousDroneCount;

    public void OnStart(ResponseObservation firstObservation, ResponseData responseData, ResponseGameInfo gameInfo)
    {
        Console.WriteLine($"[{DateTime.Now:T}] OnStart {firstObservation.Observation.GameLoop}");

        _enemyStartingLocation = gameInfo.StartRaw.StartLocations.Last();
    }

    public (List<Action>, List<DebugCommand>) OnFrame(ResponseObservation observation)
    {
        if (observation.Observation.GameLoop <= 5 || observation.Observation.GameLoop % 100 == 0)
            Console.WriteLine($"[{DateTime.Now:T}] OnFrame {observation.Observation.GameLoop}");

        var actions = new List<Action>();
        var debugCommands = new List<DebugCommand>();

        actions.Add(MorphLarva(observation, Ability.TRAIN_DRONE));
        var droneCount = observation.Observation.RawData.Units.Count(u => u.UnitType.Is(UnitType.ZERG_DRONE));
        if (droneCount > _previousDroneCount) Console.WriteLine($"[{DateTime.Now:T}] Drone count {droneCount}");

        if (droneCount >= 14)
        {
            actions.Add(AttackWithAll(observation, UnitType.ZERG_DRONE, _enemyStartingLocation));
            if (!_isAttacking) Console.WriteLine($"[{DateTime.Now:T}] Attacking {_enemyStartingLocation.ToString()}");

            _isAttacking = true;
        }

        _previousDroneCount = droneCount;

        return (actions, debugCommands);
    }

    public void OnEnd(ResponseObservation observation, Result result)
    {
        Console.WriteLine($"[{DateTime.Now:T}] OnEnd {observation.Observation.GameLoop} - Result: {result}");
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
            command.AbilityId = (int) Abilities.ATTACK;
            command.TargetWorldSpacePos = target;

            return new() {ActionRaw = new() {UnitCommand = command}};
        }

        return new() {ActionRaw = new()};
    }
}