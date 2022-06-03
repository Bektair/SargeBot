using SargeBot.Features.Debug;
using SargeBot.Features.GameData;
using SargeBot.Features.GameInfo;
using SargeBot.Features.Macro;
using SC2APIProtocol;
using SC2ClientApi;
using Action = SC2APIProtocol.Action;

namespace SargeBot.GameClients;

public class GameEngineTwo : IGameEngine
{
    private readonly DataRequestManager _dataRequestManager;
    private readonly DebugService _debugService;
    private readonly MacroManager _macroManager;
    private readonly MapService _mapService;
    private readonly GameData _gameData;

    public GameEngineTwo(DebugService debugService, MacroManager macroManager, MapService mapService, DataRequestManager dataRequestManager, GameData gameData)
    {
        _debugService = debugService;
        _macroManager = macroManager;
        _mapService = mapService;
        _dataRequestManager = dataRequestManager;
        _gameData = gameData;
    }

    public void OnStart(ResponseGameInfo gameInfo, string dataFileName="", ResponseData? responseData=null)
    {
        _mapService.PopulateMapData(gameInfo);

        if (responseData != null)
        {
            _dataRequestManager.CreateData(responseData, dataFileName);
        }
         _dataRequestManager.LoadData(); //Loads gameDataObject

        Console.WriteLine("Start game engine");
    }

    public (List<Action>, List<DebugCommand>) OnFrame(ResponseObservation observation)
    {
        Console.WriteLine($"Frame {observation.Observation.GameLoop}");

        var actions = new List<Action>();
        var debugCommands = new List<DebugCommand>();

        debugCommands.Add(_debugService.DrawText($"Frame {observation.Observation.GameLoop}"));

        actions.Add(_macroManager.BuildProbe(observation));

        return (actions, debugCommands);
    }
}