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
    private readonly MacroManager _macroManager;
    private readonly MapService _mapService;

    public GameEngine(MacroManager macroManager, MapService mapService, DataRequestManager dataRequestManager)
    {
        _macroManager = macroManager;
        _mapService = mapService;
        _dataRequestManager = dataRequestManager;
    }

<<<<<<< HEAD
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
=======
    public void OnStart(ResponseGameInfo gameInfo, string dataFileName = "", ResponseData? responseData = null)
    {
        Console.WriteLine("Start game engine");

        _mapService.PopulateMapData(gameInfo);

        if (responseData != null) _dataRequestManager.CreateData(responseData, dataFileName);
        _dataRequestManager.LoadData(); //Loads gameDataObject
>>>>>>> d34775f7670d4b5fd52499e43c88275dc255d307
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

        actions.Add(_macroManager.BuildProbe(observation));

        return (actions, debugCommands);
    }
}