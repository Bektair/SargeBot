using Core.Terran.BuildStates;
using SC2APIProtocol;

namespace Core.Terran.Bots;

public class MarineRushBot : TerranBot
{
    public MarineRushBot(IServiceProvider services) : base(services)
    {
        BuildStates = new List<BaseBuildState> { new TerranNoGasStartBuildState(this), new MarineRushBuildState(this) };
    }

    protected override void OnStart(ResponseObservation firstObs, ResponseData data, ResponseGameInfo gameInfo)
    {
        base.OnStart(firstObs, data, gameInfo);

        MessageService.Chat("GL HF");
    }
}