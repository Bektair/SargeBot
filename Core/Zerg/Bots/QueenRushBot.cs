using Core.Zerg.BuildStates;
using SC2APIProtocol;

namespace Core.Zerg.Bots;

public class QueenRushBot : ZergBot
{
    public QueenRushBot(IServiceProvider services) : base(services)
    {
        var opening = new QueenRushBuildState(this);
        BuildStates = new List<BaseBuildState> { opening };
    }

    protected override void OnStart(ResponseObservation firstObs, ResponseData data, ResponseGameInfo gameInfo)
    {
        base.OnStart(firstObs, data, gameInfo);

        MessageService.Chat("GL HF");
    }

    protected override void OnFrame(ResponseObservation observation)
    {
        base.OnFrame(observation);

        if (Intel.GameLoop % 100 == 0)
        {
            var pos = Intel.GetUnits(UnitType.ZERG_CREEPTUMOR).LastOrDefault()?.Point ?? Intel.Colonies.First().Point;
            MicroService.Order(Ability.Build_CreepTumor_Queen, Intel.GetUnits(UnitType.ZERG_QUEEN), BuildingPlacement.Random(pos));
        }
    }
}