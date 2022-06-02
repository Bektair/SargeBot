using System.Diagnostics;
using SC2APIProtocol;

namespace SC2ClientApi;

public class GameClient
{
    private readonly GameConnection _connection;
    private readonly IGameEngine _gameEngine;
    private readonly GameSettings _settings;

    public GameClient(GameSettings settings, IGameEngine gameEngine, bool asHost)
    {
        _connection = new();
        _settings = settings;
        _gameEngine = gameEngine;
        IsHost = asHost;
    }

    public bool IsHost { get; private set; }

    public Status Status => _connection.Status;

    public async Task Initialize()
    {
        if (!await ConnectToActiveClient())
        {
            LaunchClient(IsHost);
            await Task.Delay(1000);
            await ConnectToClient();
        }
    }

    public async Task Run()
    {
        var gameInfoResponse = await GameInfoRequest();
        _gameEngine.OnStart(gameInfoResponse.GameInfo);

        var observationResponse = await ObservationRequest();

        var request = _gameEngine.OnFrame(observationResponse.Observation);

        var response = await SendAndReceive(request);
        var request2 = _gameEngine.OnFrame(response.Observation);
    }

    private Process? LaunchClient(bool asHost)
    {
        IsHost = asHost;
        return Process.Start(new ProcessStartInfo(_settings.ExecutableClientPath())
        {
            Arguments = _settings.ToArguments(IsHost), WorkingDirectory = _settings.WorkingDirectory()
        });
    }

    public async Task<bool> ConnectToClient() => await _connection.Connect(_settings.GetUri(IsHost));
    public async Task<bool> ConnectToActiveClient() => await _connection.Connect(_settings.GetUri(IsHost), 1);
    public async Task Disconnect() => await _connection.Disconnect();

    public async Task<Response> SendAndReceive(Request r) => await _connection.SendAndReceiveAsync(r);
    public async Task SendRequest(Request r) => await _connection.SendAsync(r);

    public async Task<Response> StepRequest() => await SendAndReceive(ClientConstants.RequestStep);
    public async Task<Response> CreateGameRequest() => await SendAndReceive(_settings.CreateGameRequest());
    public async Task<Response> JoinGameRequest() => await SendAndReceive(_settings.JoinGameRequest(IsHost));
    public async Task<Response> RestartGameRequest() => await SendAndReceive(ClientConstants.RequestRestartGame);
    public async Task<Response> LeaveGameRequest() => await SendAndReceive(ClientConstants.RequestLeaveGame);
    public async Task<Response> QuickSaveRequest() => await SendAndReceive(ClientConstants.RequestQuickSave);
    public async Task<Response> QuickLoadRequest() => await SendAndReceive(ClientConstants.RequestQuickLoad);
    public async Task<Response> QuitRequest() => await SendAndReceive(ClientConstants.RequestQuit);
    public async Task<Response> GameInfoRequest() => await SendAndReceive(ClientConstants.RequestGameInfo);
    public async Task<Response> SaveReplayRequest() => await SendAndReceive(ClientConstants.RequestSaveReplay);
    public async Task<Response> AvailableMapsRequest() => await SendAndReceive(ClientConstants.RequestAvailableMaps);
    public async Task<Response> PingRequest() => await SendAndReceive(ClientConstants.RequestPing);
    public async Task<Response> ObservationRequest() => await SendAndReceive(ClientConstants.RequestObservation);
    public async Task<Response> DataRequest() => await SendAndReceive(ClientConstants.RequestData);
}