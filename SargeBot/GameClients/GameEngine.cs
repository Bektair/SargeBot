using SargeBot.Features.Debug;
using SargeBot.Features.GameData;
using SargeBot.Features.GameInfo;
using SargeBot.Features.Macro;
using SC2APIProtocol;
using SC2ClientApi;
using Action = SC2APIProtocol.Action;

namespace SargeBot.GameClients;

public class GameEngine : IGameEngine
{
    private readonly DataRequestManager _dataRequestManager;
    private readonly DebugService _debugService;
    private readonly MacroManager _macroManager;
    private readonly MapService _mapService;

    public GameEngine(DebugService debugService, MacroManager macroManager, MapService mapService, DataRequestManager dataRequestManager)
    {
        _debugService = debugService;
        _macroManager = macroManager;
        _mapService = mapService;
        _dataRequestManager = dataRequestManager;
    }

    /// <summary>
    /// Can be called before status ingame to use cache
    /// And after status ingame to use response
    /// Should Populate both MapData and GameData
    /// </summary>
    /// <param name="dataFileName"></param>
    /// <param name="gameInfo"></param>
    /// <param name="responseData"></param>
    public void OnStart(string dataFileName= "", ResponseGameInfo? gameInfo = null, ResponseData? responseData = null)
    {
        if (gameInfo != null) {
            _mapService.PopulateMapData(gameInfo);
            if(responseData != null)
                if (dataFileName != string.Empty) //To create file as future cache you need a name
                    _dataRequestManager.CreateLoadData(responseData, dataFileName);
                else //backup solution if the fileName was not gathered(possibly due to failing ping request)
                    _dataRequestManager.LoadDataFromResponse(responseData);
        }
        else {  //It is before status ingame, load from file
            _dataRequestManager.LoadDataFromFile(dataFileName);
        }
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