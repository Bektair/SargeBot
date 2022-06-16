using System.Diagnostics;
using SC2APIProtocol;
using SC2ClientApi.Constants;
using Action = SC2APIProtocol.Action;

namespace SC2ClientApi;

public class GameClient
{
    private readonly GameConnection _connection;
    private readonly IGameEngine _gameEngine;
    private readonly GameSettings _gs;

    public GameClient(GameSettings gs, IGameEngine gameEngine, bool isHost)
    {
        _connection = new();
        _gs = gs;
        _gameEngine = gameEngine;
        IsHost = isHost;
    }

    public bool IsHost { get; private set; }

    public Status Status => _connection.Status;
    public uint GameLoop { get; set; }

    public async Task Initialize()
    {
        if (!await ConnectToClient(1))
        {
            LaunchClient();
            await Task.Delay(5000);
            await ConnectToClient(20);
        }
    }

    public async Task Run(uint playerId)
    {
        while (_connection.Status != Status.InGame)
        {
            // wait for game to start
        }

        var gameInfoResponse = await GetGameInfo();
        var gameDataResponse = await GetStaticGameData();
        var observationResponse = await ObservationRequest();
        GameLoop = observationResponse.Observation.Observation.GameLoop;
        _gameEngine.OnStart(observationResponse.Observation, gameDataResponse, gameInfoResponse);

        await Task.Delay(1000);

        while (true)
        {
            // check in to progress the game
            await StepRequest();

            observationResponse = await ObservationRequest();
            if (observationResponse == null)
            {
                Console.WriteLine($"[{DateTime.Now:T}] Observation null. Request for next game loop manually");
                observationResponse = await ObservationRequest(GameLoop + 1);
            }

            if (observationResponse == null)
            {
                Console.WriteLine($"[{DateTime.Now:T}] Observation null again. Running next iteration");
                continue;
            }

            GameLoop = observationResponse.Observation.Observation.GameLoop;

            if (observationResponse.Status != Status.InGame)
            {
                if (observationResponse.Observation.PlayerResult != null && observationResponse.Observation.PlayerResult.Any()) Console.WriteLine($"[{DateTime.Now:T}] Observation result {observationResponse.Observation.PlayerResult.ToString()}");

                var result = observationResponse switch
                {
                    null => Result.Undecided,
                    {Status: Status.Ended} or {Status: Status.Quit} => observationResponse.Observation.PlayerResult[(int) playerId - 1].Result,
                    _ => Result.Tie
                };

                _gameEngine.OnEnd(observationResponse.Observation, result);

                break;
            }

            var (actions, debugCommands) = _gameEngine.OnFrame(observationResponse.Observation);

            await ActionRequest(actions);
            await DebugRequest(debugCommands);
        }
    }

    private async Task<ResponseGameInfo?> GetGameInfo()
    {
        var response = await GameInfoRequest();
        return response.GameInfo;
    }

    private async Task<ResponseData?> GetStaticGameData()
    {
        var response = await DataRequest();
        return response.Data;
    }

    public void LaunchClient()
    {
        LaunchClient(_gs.ExecutableClientPath(), _gs.ToArguments(IsHost), _gs.WorkingDirectory());
    }

    public Process? LaunchClient(string sc2ExecutablePath, string arguments, string workingDirectory)
    {
        Console.WriteLine($"[{DateTime.Now:T}] Starting sc2 process with arguments: {arguments}");
        return Process.Start(new ProcessStartInfo(sc2ExecutablePath)
        {
            Arguments = arguments, WorkingDirectory = workingDirectory
        });
    }

    public async Task<bool> ConnectToClient(int maxAttempts) => await _connection.Connect(_gs.ConnectionAddress, _gs.GetPort(IsHost), maxAttempts);

    public async Task Disconnect() => await _connection.Disconnect();

    public async Task<Response> SendAndReceive(Request r) => await _connection.SendAndReceiveAsync(r);
    public async Task SendRequest(Request r) => await _connection.SendAsync(r);

    public async Task<Response> StepRequest() => await SendAndReceive(ClientConstants.RequestStep);

    public async Task<Response> CreateGameRequest()
    {
        var request = _gs.CreateGameRequest();
        Console.WriteLine($"[{DateTime.Now:T}] Creating game \n{request.CreateGame.ToString()}");
        var response = await SendAndReceive(request);
        if (response.CreateGame == null)
            Console.WriteLine($"[{DateTime.Now:T}] Creating game failed \n{response.Error.ToString()}");
        else
            Console.WriteLine($"[{DateTime.Now:T}] Created game \n{response.CreateGame.ToString()}");

        return response;
    }

    public async Task<Response> JoinGameRequest() => await SendAndReceive(_gs.JoinGameRequest(IsHost));

    public async Task<Response> JoinLadderGameRequest(Race race, int startPort)
    {
        var request = JoinGameRequest(race, startPort);
        Console.WriteLine($"[{DateTime.Now:T}] Joining game \n{request.JoinGame.ToString()}");
        return await SendAndReceive(request);
    }

    public async Task<Response> RestartGameRequest() => await SendAndReceive(ClientConstants.RequestRestartGame);
    public async Task<Response> LeaveGameRequest() => await SendAndReceive(ClientConstants.RequestLeaveGame);
    public async Task<Response> QuickSaveRequest() => await SendAndReceive(ClientConstants.RequestQuickSave);
    public async Task<Response> QuickLoadRequest() => await SendAndReceive(ClientConstants.RequestQuickLoad);
    public async Task<Response> QuitRequest() => await SendAndReceive(ClientConstants.RequestQuit);
    public async Task<Response> GameInfoRequest() => await SendAndReceive(ClientConstants.RequestGameInfo);
    public async Task<Response> SaveReplayRequest() => await SendAndReceive(ClientConstants.RequestSaveReplay);
    public async Task<Response> AvailableMapsRequest() => await SendAndReceive(ClientConstants.RequestAvailableMaps);
    public async Task<Response> PingRequest() => await SendAndReceive(ClientConstants.RequestPing);

    public async Task<Response?> ObservationRequest(uint? gameLoop = null)
    {
        var obsRequest = ClientConstants.RequestObservation;

        if (gameLoop != null)
            obsRequest.Observation.GameLoop = gameLoop.Value;

        return await SendAndReceive(obsRequest);
    }

    public async Task<Response> DataRequest() => await SendAndReceive(ClientConstants.RequestData);

    public async Task<Response?> ActionRequest(List<Action> actions)
    {
        if (actions.Count == 0) return null;

        var actionRequest = new Request {Action = new()};
        actionRequest.Action.Actions.AddRange(actions);
        return await SendAndReceive(actionRequest);
    }

    private async Task<Response?> DebugRequest(List<DebugCommand> debugCommands)
    {
        if (debugCommands.Count == 0) return null;

        return await SendAndReceive(new() {Debug = new() {Debug = {debugCommands}}});
    }


    private Request JoinGameRequest(Race race, int startPort = 0)
    {
        var joinGame = new RequestJoinGame();
        joinGame.Race = race;

        if (startPort > 0)
        {
            joinGame.SharedPort = startPort + 1;
            joinGame.ServerPorts = new();
            joinGame.ServerPorts.GamePort = startPort + 2;
            joinGame.ServerPorts.BasePort = startPort + 3;

            joinGame.ClientPorts.Add(new PortSet());
            joinGame.ClientPorts[0].GamePort = startPort + 4;
            joinGame.ClientPorts[0].BasePort = startPort + 5;
        }


        joinGame.Options = new();
        joinGame.Options.FeatureLayer = new() {CropToPlayableArea = true, AllowCheatingLayers = false, MinimapResolution = new() {X = 16, Y = 16}, Resolution = new() {X = 128, Y = 128}, Width = 10};
        joinGame.Options.Raw = true;
        joinGame.Options.Score = true;
        joinGame.Options.ShowCloaked = true;
        joinGame.Options.ShowBurrowedShadows = true;
        joinGame.Options.RawCropToPlayableArea = true;
        joinGame.Options.RawAffectsSelection = true;

        var request = new Request();
        request.JoinGame = joinGame;

        return request;
    }
}