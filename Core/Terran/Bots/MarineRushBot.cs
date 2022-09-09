using Core.Terran.BuildStates;
using SC2APIProtocol;

namespace Core.Terran.Bots;

public class MarineRushBot : TerranBot
{
    public MarineRushBot(IServiceProvider services) : base(services)
    {
        var opening = new TerranNoGasStartBuildState(this);

        var main = new MarineRushBuildState(this);

        var counter = new ScvRushBuildState(this, () => Intel.GetUnits(UnitType.PROTOSS_ZEALOT, Alliance.Enemy).Any(), () => Intel.GetUnits(UnitType.ZERG_ZERGLING, Alliance.Enemy).Any());
        
        BuildStates = new List<BaseBuildState> { opening, main, counter };
    }

    protected override void OnStart(ResponseObservation firstObs, ResponseData data, ResponseGameInfo gameInfo)
    {
        base.OnStart(firstObs, data, gameInfo);

        MessageService.Chat("GL HF");
    }
}