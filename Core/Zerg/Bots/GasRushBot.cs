using Core.Terran.BuildStates;
using SC2APIProtocol;

namespace Core.Zerg.Bots;

public class GasRushBot : ZergBot
{
  public GasRushBot(IServiceProvider services) : base(services)
  {
    var opening = new GasRushBuildState(this);
    BuildStates = new List<BaseBuildState> { opening };

  }

  protected override void OnStart(ResponseObservation firstObs, ResponseData data, ResponseGameInfo gameInfo)
  {
    base.OnStart(firstObs, data, gameInfo);

    MessageService.Chat("GL HF");
  }
}