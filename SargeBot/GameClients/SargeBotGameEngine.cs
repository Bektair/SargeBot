using SargeBot.Features.Debug;
using SargeBot.Features.GameData;
using SargeBot.Features.GameInfo;
using SargeBot.Features.Intel;
using SargeBot.Features.Macro;
using SargeBot.Features.Micro;
using SC2APIProtocol;
using SC2ClientApi;
using Action = SC2APIProtocol.Action;

namespace SargeBot.GameClients;

public class SargeBotGameEngine : IGameEngine
{
    private readonly IntelService _intelService;
    private readonly MacroManager _macroManager;
    private readonly MapDataService _mapService;
    private readonly MicroManager _microManager;
    private readonly StaticGameData _staticGameData;

    public SargeBotGameEngine(IntelService intelService, MacroManager macroManager, MapDataService mapService, MicroManager microManager, StaticGameData staticGameData)
    {
        _intelService = intelService;
        _macroManager = macroManager;
        _mapService = mapService;
        _microManager = microManager;
        _staticGameData = staticGameData;
    }

    public void OnStart(ResponseObservation firstObservation, ResponseData responseData, ResponseGameInfo gameInfo)
    {
        _intelService.OnStart(firstObservation, responseData, gameInfo);
        _staticGameData.PopulateGameData(responseData);
        Task.Run(() => _staticGameData.Save());
        _mapService.PopulateMapData(gameInfo);
    }

    public (List<Action>, List<DebugCommand>) OnFrame(ResponseObservation observation)
    {
        if (observation.Observation.GameLoop % 100 == 0)
            Console.WriteLine($"Frame {observation.Observation.GameLoop}");

        var actions = new List<Action>();
        var debugCommands = new List<DebugCommand>();

        _intelService.OnFrame(observation);

        var enemyBase = _intelService.EnemyColonies.First();

        // testing debug commands
        var z = 12;
        debugCommands.Add(DebugService.DrawText($"Frame {observation.Observation.GameLoop}"));
        // debugCommands.Add(DebugService.DrawSphere(new() {X = 14.5f, Y = 24.5f, Z = z}, color: new() {G = 255}));
        // debugCommands.Add(DebugService.DrawSphere(new() {X = 113.5f, Y = 123.5f, Z = z}, color: new() {R = 255, B = 255}));
        // debugCommands.Add(DebugService.DrawLine(new() {X = 0, Y = 0, Z = z}, new() {X = 255, Y = 255, Z = z}, new() {R = 255}));
        // debugCommands.Add(DebugService.DrawBox(new() {X = 15, Y = 15, Z = z}, new() {X = 100, Y = 100, Z = z}, new() {B = 255}));
        debugCommands.Add(DebugService.DrawBox(new() {X = enemyBase.Point.X, Y = enemyBase.Point.Y, Z = z}, new() {X = enemyBase.Point.X + 5, Y = enemyBase.Point.Y + 5, Z = z}, new() {B = 255}));
        if (_intelService.SelfNatural != null)
            debugCommands.Add(DebugService.DrawSphere(new() {X = _intelService.SelfNatural.X, Y = _intelService.SelfNatural.Y, Z = z}, color: new() {G = 255}));

        var canAffordSpawningPool = observation.Observation.PlayerCommon.Minerals >= 200;
        var hasSpawningPool = observation.Observation.RawData.Units.Any(u => u.UnitType.Is(UnitType.ZERG_SPAWNINGPOOL));
        if (canAffordSpawningPool && !hasSpawningPool)
            actions.Add(_macroManager.BuildSpawningPool(observation));

        actions.Add(_microManager.OverlordScout(observation));


        var eggCount = observation.Observation.RawData.Units.Count(u => u.UnitType.Is(UnitType.ZERG_EGG));
        if (eggCount == 0 && !hasSpawningPool) actions.Add(MacroManager.MorphLarva(observation, Ability.TRAIN_DRONE));
        if (eggCount == 0 && hasSpawningPool && observation.Observation.PlayerCommon.FoodUsed == 14) actions.Add(MacroManager.MorphLarva(observation, Ability.TRAIN_OVERLORD));

        var droneCount = observation.Observation.RawData.Units.Count(u => u.UnitType.Is(UnitType.ZERG_DRONE));
        var lingCount = observation.Observation.RawData.Units.Count(u => u.UnitType.Is(UnitType.ZERG_ZERGLING));
        if (lingCount <= 6 || droneCount > 20) actions.Add(MacroManager.MorphLarva(observation, Ability.TRAIN_ZERGLING));
        else actions.Add(MacroManager.MorphLarva(observation, Ability.TRAIN_DRONE));

        _microManager.AttackWithAll(observation, UnitType.ZERG_ZERGLING, enemyBase.Point);
        if (lingCount >= 6)
        {
            actions.Add(_microManager.AttackWithAll(observation, UnitType.ZERG_DRONE, enemyBase.Point));
        }

        return (actions, debugCommands);
    }

    public void OnEnd(ResponseObservation? observation)
    {
        if (observation == null) return;
        Log.Info($"OnEnd {observation.Observation.GameLoop} {observation.PlayerResult.ToString()}");
    }
}