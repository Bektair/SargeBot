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

    public void OnStart(ResponseGameInfo gameInfo)
    {
        Console.WriteLine("Start game engine");
    }

    public (List<Action>, List<DebugCommand>) OnFrame(ResponseObservation observation)
    {
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