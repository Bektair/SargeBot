using Core.Data;
using Core.Intel;
using Microsoft.Extensions.DependencyInjection;
using SC2APIProtocol;
using SC2ClientApi;

namespace Core;

public abstract class BaseBot
{
    protected readonly IDataService Data;
    protected readonly IIntelService Intel;
    protected readonly IMacroService MacroService;
    protected readonly IMessageService MessageService;
    protected readonly IMicroService MicroService;

    protected BaseBot(IServiceProvider services, Race race)
    {
        Data = services.GetRequiredService<IEnumerable<IDataService>>().First(x => x.Race == race);
        Intel = services.GetRequiredService<IEnumerable<IIntelService>>().First(x => x.Race == race);
        MacroService = services.GetRequiredService<IEnumerable<IMacroService>>().First(x => x.Race == race);
        MessageService = services.GetRequiredService<IMessageService>();
        MicroService = services.GetRequiredService<IMicroService>();
        PlayerSetup = new PlayerSetup { PlayerName = GetType().Name, Race = race, Type = PlayerType.Participant };
    }

    public PlayerSetup PlayerSetup { get; }

    protected virtual void OnStart(ResponseObservation firstObs, ResponseData data, ResponseGameInfo gameInfo)
    {
        Data.OnStart(firstObs, data, gameInfo);
        Intel.OnStart(firstObs, data, gameInfo);
    }

    protected virtual void OnFrame(ResponseObservation observation)
    {
        Intel.OnFrame(observation);
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

            var obs = await gameConnection.Observation();
            // if (obs == null || gameConnection.Status == Status.Ended) break;

            OnFrame(obs);
        }

        OnEnd();
    }
}