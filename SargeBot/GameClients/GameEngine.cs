﻿using SargeBot.Features.Debug;
using SargeBot.Features.GameData;
using SargeBot.Features.GameInfo;
using SargeBot.Features.Intel;
using SargeBot.Features.Macro;
using SargeBot.Features.Micro;
using SC2APIProtocol;
using SC2ClientApi;
using SC2ClientApi.Constants;
using Action = SC2APIProtocol.Action;

namespace SargeBot.GameClients;

public class GameEngine : IGameEngine
{
    private readonly DataRequestManager _dataRequestManager;
    private readonly IntelService _intelService;
    private readonly MacroManager _macroManager;
    private readonly MapDataService _mapService;
    private readonly MicroManager _microManager;

    public GameEngine(MacroManager macroManager, MapDataService mapService, DataRequestManager dataRequestManager, IntelService intelService, MicroManager microManager)
    {
        _macroManager = macroManager;
        _mapService = mapService;
        _dataRequestManager = dataRequestManager;
        _intelService = intelService;
        _microManager = microManager;
    }

    /// <summary>
    /// Can be called before status ingame to use cache
    /// And after status ingame to use response
    /// Should Populate both MapData and GameData
    /// </summary>
    public void OnStart(ResponseObservation firstObservation, ResponseData? responseData = null, ResponseGameInfo? gameInfo = null, string mapName = "")
    {
        _intelService.OnStart(firstObservation, responseData, gameInfo);

        if(responseData != null)
            if(!_dataRequestManager.CreateLoadData(responseData))
                _dataRequestManager.LoadDataFromResponse(responseData);
        if (gameInfo != null)
            _mapService.CreateLoadFile(gameInfo, mapName);
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
        var hasSpawningPool = observation.Observation.RawData.Units.Any(u => u.UnitType.Is(UnitTypes.ZERG_SPAWNINGPOOL));
        if (canAffordSpawningPool && !hasSpawningPool)
            actions.Add(_macroManager.BuildSpawningPool(observation));

        actions.Add(_microManager.OverlordScout(observation));

        var lingCount = observation.Observation.RawData.Units.Count(u => u.UnitType.Is(UnitTypes.ZERG_ZERGLING));
        if (lingCount <= 6) actions.Add(MacroManager.MorphLarva(observation, Abilities.TRAIN_ZERGLING));

        actions.Add(_microManager.ZerglingAttack(observation));

        return (actions, debugCommands);
    }

    public void LoadFromCache(string gameMap, bool shouldLoadDataCache, bool shouldLoadInfoCache)
    {
        if(shouldLoadDataCache)
            _dataRequestManager.LoadDataFromFile();
        if (shouldLoadInfoCache)
            _mapService.LoadDataFromFile(gameMap);
    }
}