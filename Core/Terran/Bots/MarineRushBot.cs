using Core.Terran.BuildStates;
using SC2APIProtocol;

namespace Core.Terran.Bots;

public class MarineRushBot : TerranBot
{
    public MarineRushBot(IServiceProvider services) : base(services)
    {
        var opening = new TerranNoGasStartBuildState(this);

        var main = new MarineRushBuildState(this);

        var counter = new ScvRushBuildState(this, () => Intel.GetEnemyUnits(UnitType.PROTOSS_ZEALOT).Any(), () => Intel.GetEnemyUnits(UnitType.ZERG_ZERGLING).Any());
        
        BuildStates = new List<BaseBuildState> { opening, main, counter };
    }

    protected override void OnStart(ResponseObservation firstObs, ResponseData data, ResponseGameInfo gameInfo)
    {
        base.OnStart(firstObs, data, gameInfo);

        MessageService.Chat("GL HF");
    }
}