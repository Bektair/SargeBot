﻿using Core.Data;
using Core.Intel;
using Microsoft.Extensions.DependencyInjection;
using SC2APIProtocol;
using SC2ClientApi;

namespace Core;

public abstract class BaseBot
{
    public readonly IDataService Data;
    public readonly IIntelService Intel;
    public readonly IMacroService MacroService;
    public readonly IMessageService MessageService;
    public readonly IMicroService MicroService;
    public readonly MapDataService MapDataService;

    protected BaseBot(IServiceProvider services, Race race)
    {
        Data = services.GetRequiredService<IEnumerable<IDataService>>().First(x => x.Race == race);
        Intel = services.GetRequiredService<IEnumerable<IIntelService>>().First(x => x.Race == race);
        MacroService = services.GetRequiredService<IEnumerable<IMacroService>>().First(x => x.Race == race);
        MessageService = services.GetRequiredService<IMessageService>();
        MicroService = services.GetRequiredService<IMicroService>();
        MapDataService = services.GetRequiredService<MapDataService>();

        PlayerSetup = new PlayerSetup { PlayerName = GetType().Name, Race = race, Type = PlayerType.Participant };
    }

    private BaseBuildState? CurrentBuildState { get; set; }
    protected IEnumerable<BaseBuildState> BuildStates { get; init; } = new List<BaseBuildState>();

    public PlayerSetup PlayerSetup { get; }


    protected virtual void OnStart(ResponseObservation firstObs, ResponseData data, ResponseGameInfo gameInfo)
    {
        Data.OnStart(firstObs, data, gameInfo);
        Intel.OnStart(firstObs, data, gameInfo, MessageService);
        CurrentBuildState = BuildStates.FirstOrDefault();
        MapDataService.OnStart(gameInfo);

    }

    protected virtual void OnFrame(ResponseObservation observation)
    {
        Intel.OnFrame(observation);
        CheckStateTriggers();
        CurrentBuildState?.OnFrame();
        MessageService.OnFrame(observation);

        var bases = Intel.BaseLocations;
        var color = new Color { G = 0, R = 250, B = 50 };
        foreach (var expansion in bases)
        {
            //You can get the Z - height of terrain return -16 + 32 * self.game_info.terrain_height[pos] / 255
            //Get data from map
            var Zpoint = new Point2D() { X = (int)Math.Round(expansion.X), Y = (int)Math.Round(expansion.Y) };

            MessageService.Debug(DebugRequest.DrawSphere(
              new Point { X = expansion.X, Y = expansion.Y, Z = MapDataService.MapData.Map[Zpoint].ZHegith },
              color: color));
        }

    }

    protected virtual void OnEnd()
    {
        Log.Info($"Game ended on loop {Intel.GameLoop}");
    }

    internal async Task Run(GameConnection gameConnection)
    {
        while (gameConnection.Status != Status.InGame)
        {
            // loading screen
        }

        MessageService.SetConnection(gameConnection);

        OnStart(
            await gameConnection.Observation(),
            await gameConnection.GameData(),
            await gameConnection.GameInfo()
        );

        while (gameConnection.Status == Status.InGame)
        {
            await gameConnection.Step();

            OnFrame(await gameConnection.Observation());
        }

        OnEnd();
    }

    private void CheckStateTriggers()
    {
        foreach (var state in BuildStates.Where(x => x != CurrentBuildState))
            if (state.ShouldActivate())
            {
                Log.Info($"Build state change {CurrentBuildState.GetType()} -> {state.GetType()}");
                CurrentBuildState = state;
                break;
            }
    }
}