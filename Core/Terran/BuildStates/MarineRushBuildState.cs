using SC2ClientApi;

namespace Core.Terran.BuildStates;

public class MarineRushBuildState : BaseBuildState
{
    private bool _attacking;

    public MarineRushBuildState(BaseBot bot, params Func<bool>[] activationTriggers) : base(bot, activationTriggers)
    {
    }

    public override void OnFrame()
    {
        if (Bot.Intel.GetUnits(UnitType.TERRAN_SUPPLYDEPOT).Count <= Bot.Intel.GetUnits(UnitType.TERRAN_BARRACKS).Count)
            Bot.MacroService.Build(UnitType.TERRAN_SUPPLYDEPOT, 2);

        if (Bot.Intel.GetUnits(UnitType.TERRAN_BARRACKS).Count < 4)
            Bot.MacroService.Build(UnitType.TERRAN_BARRACKS);

        var simpleRamp = Bot.Intel.Colonies.First().Point
            .GetPointBetweenPoints(Bot.Intel.EnemyColonies.First().Point, -7);

        Bot.MacroService.Train(UnitType.TERRAN_MARINE, simpleRamp);
        
        if (Bot.Intel.GetUnits(UnitType.TERRAN_MARINE).Count <= 10)
        {
            if (_attacking)
                Log.Warning("Stopped attacking");
            _attacking = false;
            return;
        }

        var enemyBase = Bot.Intel.EnemyColonies.First();

        var attackers = new Squad();
        attackers.AddUnits(Bot.Intel.GetUnits(UnitType.TERRAN_MARINE));

        Bot.MicroService.AttackMove(attackers, enemyBase.Point);

        if (!_attacking || Bot.Intel.GameLoop % 500 == 0)
            Log.Success($"Attacking with {attackers.Units.Count} {(UnitType?)attackers.Units.FirstOrDefault()?.UnitType}");
        _attacking = true;
    }

    protected override Func<bool> DefaultTrigger(BaseBot bot) => () => bot.Intel.GetUnits(UnitType.TERRAN_BARRACKS).Any() && bot.Intel.GameLoop < 2000;
}