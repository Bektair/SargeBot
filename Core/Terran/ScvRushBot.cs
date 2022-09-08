using SC2APIProtocol;
using SC2ClientApi;

namespace Core.Terran;

public class ScvRushBot : TerranBot
{
    public ScvRushBot(IServiceProvider services) : base(services)
    {
    }

    protected override void OnFrame(ResponseObservation observation)
    {
        base.OnFrame(observation);

        MacroService.Train(UnitType.TERRAN_SCV);

        if (Intel.GetWorkers().Count < 14) return;

        var enemyBase = Intel.EnemyColonies.First();

        var attackers = new Squad();
        attackers.AddUnits(Intel.GetWorkers());

        MicroService.AttackMove(attackers, enemyBase.Point);

        Log.Warning($"Attacking with {attackers.Units.Count} {attackers.Units.FirstOrDefault()?.UnitType}");
    }
}