using Core.Zerg;
using SC2ClientApi;

namespace Core.Terran.BuildStates;

public class GasRushBuildState : BaseBuildState
{
    private bool _attacking = true;
    private ZergMacroService service;

    public GasRushBuildState(BaseBot bot, params Func<bool>[] activationTriggers) : base(bot, activationTriggers)
    {
      service = (ZergMacroService)Bot.MacroService;

    }

  public override void OnFrame()
  {

      service.Build(UnitType.ZERG_EXTRACTOR, 1);

/*
    if(Bot.Intel.)
    service.Build(UnitType.ZERG_SPAWNINGPOOL, 1);*/


  }

  protected override Func<bool> DefaultTrigger(BaseBot bot) => () => bot.Intel.GetUnits(UnitType.TERRAN_BARRACKS).Any() && bot.Intel.GameLoop < 2000;
}