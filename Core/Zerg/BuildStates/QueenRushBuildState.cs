namespace Core.Zerg.BuildStates;

public class QueenRushBuildState : BaseBuildState
{
    private readonly ZergMacroService _zergMacroService;

    public QueenRushBuildState(BaseBot bot, params Func<bool>[] activationTriggers) : base(bot, activationTriggers)
    {
        _zergMacroService = (ZergMacroService)Bot.MacroService;
    }

    public override void OnFrame()
    {
        _zergMacroService.Build(UnitType.ZERG_HATCHERY, 1);

        if (Bot.Intel.SupplyAvailable <= 2)
        {
            _zergMacroService.Train(UnitType.ZERG_OVERLORD);
            return;
        }

        if (Bot.Intel.GetUnits(UnitType.ZERG_SPAWNINGPOOL).Count == 0)
        {
            _zergMacroService.Build(UnitType.ZERG_SPAWNINGPOOL, 1);
            _zergMacroService.Train(UnitType.ZERG_DRONE);
        }
        else
            _zergMacroService.Train(UnitType.ZERG_QUEEN);
    }

    protected override Func<bool> DefaultTrigger(BaseBot bot)
    {
        return () => false;
    }
}