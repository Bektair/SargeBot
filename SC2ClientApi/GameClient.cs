using System.Diagnostics;
using System.Net;
using SC2APIProtocol;

namespace SC2ClientApi;

public class GameClient
{
    private readonly GameConnection _connection;
    private readonly GameSettings _settings;
    private bool _isHost;

    public GameClient()
    {
        _connection = new();
        _settings = new()
        {
            FolderPath = @"C:\Program Files (x86)\StarCraft II",
            ConnectionAddress = IPAddress.Loopback.ToString(),
            ConnectionServerPort = 8165,
            ConnectionClientPort = 8170,
            MultiplayerSharedPort = 8175,
            InterfaceOptions = new() {Raw = true, Score = true},
            Fullscreen = false,
            ClientWindowWidth = 1024,
            ClientWindowHeight = 768,
            GameMap = "Cloud Kingdom LE",
            Realtime = false,
            DisableFog = false,
            ParticipantRace = Race.Protoss,
            Opponents = new() {new() {Type = PlayerType.Computer, Race = Race.Random, Difficulty = Difficulty.VeryEasy}}
        };
    }

    public GameClient(GameSettings settings)
    {
        _connection = new();
        _settings = settings;
    }

    public Status Status => _connection.Status;

    public async Task Initialize(bool asHost)
    {
        _isHost = asHost;
        if (!await ConnectToActiveClient())
        {
            LaunchClient(asHost);
            await ConnectToClient();
        }
    }

    public Process? LaunchClient(bool asHost)
    {
        _isHost = asHost;
        return Process.Start(new ProcessStartInfo(_settings.ExecutableClientPath())
        {
            Arguments = _settings.ToArguments(_isHost), WorkingDirectory = _settings.WorkingDirectory()
        });
    }

    public async Task<bool> ConnectToClient() => await _connection.Connect(_settings.GetUri(_isHost));
    public async Task<bool> ConnectToActiveClient() => await _connection.Connect(_settings.GetUri(_isHost), 1);
    public async Task Disconnect() => await _connection.Disconnect();

    public async Task<Response> SendAndReceive(Request r) => await _connection.SendAndReceiveAsync(r);
    public async Task SendRequest(Request r) => await _connection.SendAsync(r);

    public async Task<Response> StepRequest() => await SendAndReceive(ClientConstants.RequestStep);
    public async Task<Response> CreateGameRequest() => await SendAndReceive(_settings.CreateGameRequest());
    public async Task<Response> JoinGameRequest() => await SendAndReceive(_settings.JoinGameRequest(_isHost));
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