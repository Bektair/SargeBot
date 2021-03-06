using SC2APIProtocol;
using SC2ClientApi.Constants;
using Action = SC2APIProtocol.Action;

namespace SC2ClientApi;

public class GameClient
{
    private readonly GameConnection _connection;
    private readonly IGameEngine _gameEngine;
    private readonly GameSettings _gs;

    public GameClient(GameSettings gs, IGameEngine gameEngine, PlayerSetup player, bool isHost)
    {
        _connection = new();
        _gs = gs;
        _gameEngine = gameEngine;
        IsHost = isHost;
        Player = player;
    }

    public PlayerSetup Player { get; }
    public bool IsHost { get; }
    public Status ConnectionStatus => _connection.Status;
    public uint GameLoop { get; set; }
    public uint PlayerId { get; set; }

    public async Task Connect()
    {
        if (!await ConnectToClient(1))
        {
            Sc2Process.Start(_gs, IsHost);
            await Task.Delay(5000);
            await ConnectToClient(20);
        }
    }

    public async Task Run()
    {
        while (ConnectionStatus != Status.InGame)
        {
            // wait for game to start
        }

        var gameInfo = await GameInfo();
        var staticGameData = await StaticGameData();
        var observation = await Observation();
        _gameEngine.OnStart(observation, staticGameData, gameInfo);

        await Task.Delay(1000);

        while (true)
        {
            // check in to progress the game
            await Step();

            observation = await Observation();
            if (observation == null)
            {
                Log.Warning($"[{DateTime.Now:T}] Observation null. Request for next game loop manually");
                observation = await Observation(GameLoop + 1);
            }

            if (observation == null || ConnectionStatus != Status.InGame)
            {
                _gameEngine.OnEnd(observation);
                break;
            }

            var (actions, debugCommands) = _gameEngine.OnFrame(observation);

            await ActionRequest(actions);
            await DebugRequest(debugCommands);
        }
    }

    public async Task<bool> ConnectToClient(int maxAttempts) => await _connection.Connect(_gs.HostAddress, _gs.GetPort(IsHost), maxAttempts);

    private async Task Step() => await _connection.SendAndReceiveAsync(ClientConstants.RequestStep);

    public async Task<ResponseCreateGame?> CreateGame()
    {
        var request = new Request
        {
            CreateGame = new()
            {
                Realtime = _gs.Realtime,
                DisableFog = _gs.DisableFog,
                PlayerSetup = {_gs.PlayerOne, _gs.PlayerTwo}
            }
        };

        if (File.Exists(_gs.MapName) || File.Exists(Sc2Process.MapDirectory(_gs.MapName)))
            request.CreateGame.LocalMap = new() {MapPath = _gs.MapName};
        else
            request.CreateGame.BattlenetMapName = _gs.MapName;

        Log.Info($"{Player.PlayerName} Creating game");
        var response = await _connection.SendAndReceiveAsync(request);
        if (response?.CreateGame == null || response.CreateGame.HasError)
            Log.Error($"Creating game failed {response?.Error} {response?.CreateGame?.Error}");
        else
            Log.Success($"{Player.PlayerName} Created game");

        return response?.CreateGame;
    }

    public async Task<ResponseJoinGame?> JoinGame()
    {
        var request = new Request
        {
            JoinGame = new()
            {
                Race = Player.Race,
                PlayerName = Player.PlayerName,
                Options = _gs.InterfaceOptions
            }
        };

        if (_gs.IsMultiplayer())
        {
            // request.JoinGame.SharedPort // deprecated: https://github.com/Blizzard/s2client-proto/blob/master/s2clientprotocol/sc2api.proto#L220
            request.JoinGame.ServerPorts = new() {GamePort = _gs.HostPort + 1, BasePort = _gs.HostPort + 2};
            request.JoinGame.ClientPorts.Add(new PortSet {GamePort = _gs.GuestPort + 1, BasePort = _gs.GuestPort + 2});
        }

        Log.Info($"{Player.PlayerName} Joining game");
        var response = await _connection.SendAndReceiveAsync(request);
        if (response?.JoinGame == null || response.JoinGame.HasError)
        {
            // getting null response, but the clients are still joining :/
            // Log.Error($"{Player.PlayerName} Joining game failed {response?.JoinGame.Error}");
            return null;
        }

        Log.Success($"{Player.PlayerName} Joined game {response.JoinGame}");
        PlayerId = response.JoinGame.PlayerId;
        return response.JoinGame;
    }

    private async Task<ResponseObservation?> Observation(uint? gameLoop = null)
    {
        var request = ClientConstants.RequestObservation;

        if (gameLoop != null)
            request.Observation.GameLoop = gameLoop.Value;

        var response = await _connection.SendAndReceiveAsync(request);
        if (response?.Observation == null)
        {
            Log.Warning("Observation null");
            return null;
        }

        GameLoop = response.Observation.Observation.GameLoop;
        return response.Observation;
    }

    private async Task<ResponseAction?> ActionRequest(List<Action> actions)
    {
        if (actions.Count == 0) return null;

        var request = new Request {Action = new()};
        request.Action.Actions.AddRange(actions);

        var response = await _connection.SendAndReceiveAsync(request);
        return response?.Action;
    }

    private async Task<ResponseDebug?> DebugRequest(List<DebugCommand> debugCommands)
    {
        if (debugCommands.Count == 0) return null;

        var response = await _connection.SendAndReceiveAsync(new() {Debug = new() {Debug = {debugCommands}}});

        return response?.Debug;
    }

    private async Task<ResponseGameInfo?> GameInfo()
    {
        var response = await _connection.SendAndReceiveAsync(ClientConstants.RequestGameInfo);
        return response?.GameInfo;
    }

    private async Task<ResponseData?> StaticGameData()
    {
        var response = await _connection.SendAndReceiveAsync(ClientConstants.RequestData);
        return response?.Data;
    }

    #region Not used

    public async Task Disconnect() => await _connection.Disconnect();
    public async Task<Response> RestartGameRequest() => await _connection.SendAndReceiveAsync(ClientConstants.RequestRestartGame);
    public async Task<Response> LeaveGameRequest() => await _connection.SendAndReceiveAsync(ClientConstants.RequestLeaveGame);
    public async Task<Response> QuickSaveRequest() => await _connection.SendAndReceiveAsync(ClientConstants.RequestQuickSave);
    public async Task<Response> QuickLoadRequest() => await _connection.SendAndReceiveAsync(ClientConstants.RequestQuickLoad);
    public async Task<Response> QuitRequest() => await _connection.SendAndReceiveAsync(ClientConstants.RequestQuit);
    public async Task<Response> SaveReplayRequest() => await _connection.SendAndReceiveAsync(ClientConstants.RequestSaveReplay);
    public async Task<Response> AvailableMapsRequest() => await _connection.SendAndReceiveAsync(ClientConstants.RequestAvailableMaps);
    public async Task<Response> PingRequest() => await _connection.SendAndReceiveAsync(ClientConstants.RequestPing);

    #endregion
}