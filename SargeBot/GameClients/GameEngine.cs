using SargeBot.Features.Debug;
using SargeBot.Features.GameData;
using SargeBot.Features.GameInfo;
using SargeBot.Features.Intel;
using SargeBot.Features.Macro;
using SC2APIProtocol;
using SC2ClientApi;
using Action = SC2APIProtocol.Action;

namespace SargeBot.GameClients;

public class GameEngine : IGameEngine
{
    private readonly DataRequestManager _dataRequestManager;
    private readonly MacroManager _macroManager;
    private readonly MapDataService _mapService;
    private readonly IntelService _intelService;

    public GameEngine(MacroManager macroManager, MapDataService mapService, DataRequestManager dataRequestManager, IntelService intelService)
    {
        _macroManager = macroManager;
        _mapService = mapService;
        _dataRequestManager = dataRequestManager;
        _intelService = intelService;
    }

    /// <summary>
    /// Can be called before status ingame to use cache
    /// And after status ingame to use response
    /// Should Populate both MapData and GameData
    /// </summary>
    public void OnStart(ResponseObservation firstObservation, ResponseData? responseData = null, ResponseGameInfo? gameInfo = null, string mapName = "")
    {
        _intelService.OnStart(firstObservation, responseData, gameInfo);
        
        if (gameInfo != null) 
            _mapService.CreateLoadFile(gameInfo, mapName);

        if(responseData != null)
            if(!_dataRequestManager.CreateLoadData(responseData))
                _dataRequestManager.LoadDataFromResponse(responseData);
    }

    public (List<Action>, List<DebugCommand>) OnFrame(ResponseObservation observation)
    {
        if (observation.Observation.GameLoop % 100 == 0)
            Console.WriteLine($"Frame {observation.Observation.GameLoop}");

        var actions = new List<Action>();
        var debugCommands = new List<DebugCommand>();

        // testing debug commands
        var z = 22;
        debugCommands.Add(DebugService.DrawText($"Frame {observation.Observation.GameLoop}"));
        debugCommands.Add(DebugService.DrawLine(new() {X = 0, Y = 0, Z = z}, new() {X = 255, Y = 255, Z = z}, new() {R = 255}));
        debugCommands.Add(DebugService.DrawSphere(new() {X = 5, Y = 5, Z = z}, color: new() {G = 255}));
        debugCommands.Add(DebugService.DrawBox(new() {X = 15, Y = 15, Z = z}, new() {X = 100, Y = 100, Z = z}, new() {B = 255}));

        actions.Add(_macroManager.BuildSpawningPool(observation));

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