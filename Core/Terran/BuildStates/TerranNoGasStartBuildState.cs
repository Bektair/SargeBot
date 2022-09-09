namespace Core.Terran.BuildStates;

public class TerranNoGasStartBuildState : BaseBuildState
{
    public TerranNoGasStartBuildState(BaseBot bot, params Func<bool>[] activationTriggers) : base(bot, activationTriggers)
    {
    }

    public override void OnFrame()
    {
        Bot.MacroService.Train(UnitType.TERRAN_SCV);

        if (Bot.Intel.GetUnits(UnitType.TERRAN_SCV).Count >= 13 && Bot.Intel.GetUnits(UnitType.TERRAN_SUPPLYDEPOT).Count == 0)
            Bot.MacroService.Build(UnitType.TERRAN_SUPPLYDEPOT);

        if (Bot.Intel.GetUnits(UnitType.TERRAN_SUPPLYDEPOT).Count == 1)
            Bot.MacroService.Build(UnitType.TERRAN_BARRACKS);
    }

    protected override Func<bool> DefaultTrigger(BaseBot bot) => () => bot.Intel.GetUnits(UnitType.TERRAN_SCV).Count < 5;
}