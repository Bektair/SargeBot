﻿using System.Diagnostics;
using SC2APIProtocol;
using SC2ClientApi.Constants;
using Action = SC2APIProtocol.Action;
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
        bool shouldLoadDataCache = LoadDataCache();
        bool shouldLoadInfoCache = LoadInfoCache();
        if (shouldLoadDataCache || shouldLoadInfoCache)
                _gameEngine.LoadFromCache(_settings.GameMap, shouldLoadDataCache, shouldLoadInfoCache);
        while (_connection.Status != Status.InGame)
        {
            // wait for game to start
        }


        ResponseGameInfo gameInfoResponse = null;
        ResponseData gameDataResponse = null;
        if (!shouldLoadInfoCache) { gameInfoResponse = (await GameInfoRequest()).GameInfo; }
        if (!shouldLoadDataCache){ gameDataResponse = (await DataRequest()).Data; }
        
        var firstObservationResponse = await ObservationRequest();
        _gameEngine.OnStart(firstObservationResponse.Observation, gameDataResponse, gameInfoResponse, _settings.GameMap);

        while (_connection.Status == Status.InGame)
        {
            // check in to progress the game
            await StepRequest();

            var observationResponse = await ObservationRequest();

            var (actions, debugCommands) = _gameEngine.OnFrame(observationResponse.Observation);

            await ActionRequest(actions);
            await DebugRequest(debugCommands);
        }
    }

    private bool LoadDataCache()
    {
        return DataFileExists(_settings.DataFileName);
    }

    private bool LoadInfoCache()
    {
        return DataFileExists(_settings.GameMap + ".json");
    }
    public bool DataFileExists(string filename)
    {
        if (File.Exists(Path.Combine(_settings.DataFolderName, filename)))
            return true;
        else return false;
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

    public async Task<Response> ActionRequest(List<Action> actions)
    {
        var actionRequest = new Request {Action = new()};
        actionRequest.Action.Actions.AddRange(actions);
        return await SendAndReceive(actionRequest);
    }

    private async Task<Response> DebugRequest(List<DebugCommand> debugCommands)
    {
        var request = new Request {Debug = new() {Debug = {debugCommands}}};
        return await SendAndReceive(request);
    }
}