using SC2ClientApi;

namespace Core.Terran.BuildStates;

public class ScvRushBuildState : BaseBuildState
{
    public ScvRushBuildState(BaseBot bot, params Func<bool>[] activationTriggers) : base(bot, activationTriggers)
    {
    }

    public override void OnFrame()
    {
        Bot.MacroService.Train(UnitType.TERRAN_SCV);

        var enemyBase = Bot.Intel.EnemyColonies.First();

        var attackers = new Squad();
        attackers.AddUnits(Bot.Intel.GetWorkers());

        Bot.MicroService.AttackMove(attackers, enemyBase.Point);

        if (Bot.Intel.GameLoop % 500 == 0)
            Log.Success($"Attacking with {attackers.Units.Count} {(UnitType?)attackers.Units.FirstOrDefault()?.UnitType}");
    }
}