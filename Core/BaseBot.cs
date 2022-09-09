using Core.Data;
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

    protected BaseBot(IServiceProvider services, Race race)
    {
        Data = services.GetRequiredService<IEnumerable<IDataService>>().First(x => x.Race == race);
        Intel = services.GetRequiredService<IEnumerable<IIntelService>>().First(x => x.Race == race);
        MacroService = services.GetRequiredService<IEnumerable<IMacroService>>().First(x => x.Race == race);
        MessageService = services.GetRequiredService<IMessageService>();
        MicroService = services.GetRequiredService<IMicroService>();
        PlayerSetup = new PlayerSetup { PlayerName = GetType().Name, Race = race, Type = PlayerType.Participant };
    }

    private BaseBuildState CurrentBuildState { get; set; }
    protected IEnumerable<BaseBuildState> BuildStates { get; init; }

    public PlayerSetup PlayerSetup { get; }

    protected virtual void OnStart(ResponseObservation firstObs, ResponseData data, ResponseGameInfo gameInfo)
    {
        Data.OnStart(firstObs, data, gameInfo);
        Intel.OnStart(firstObs, data, gameInfo);
        CurrentBuildState = BuildStates.First();
    }

    protected virtual void OnFrame(ResponseObservation observation)
    {
        Intel.OnFrame(observation);
        CheckStatePredicates();
        CurrentBuildState.OnFrame();
        MessageService.OnFrame();
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

    private void CheckStatePredicates()
    {
        foreach (var state in BuildStates.Where(x => x != CurrentBuildState))
        {
            if (state.NextState())
            {
                Log.Info($"Build state change {CurrentBuildState.GetType()} -> {state.GetType()}");
                CurrentBuildState = state;
                break;
            }
        }
    }
}