using SC2APIProtocol;
using SC2ClientApi;

namespace Core.Protoss;

public class ProbeRushBot : ProtossBot
{
    public ProbeRushBot(IServiceProvider services) : base(services)
    {
    }

    protected override void OnFrame(ResponseObservation observation)
    {
        base.OnFrame(observation);

        MacroService.Train(UnitType.PROTOSS_PROBE);

        if (Intel.GetWorkers().Count < 14) return;

        var enemyBase = Intel.EnemyColonies.First();

        var attackers = new Squad();
        attackers.AddUnits(Intel.GetWorkers());

        MicroService.AttackMove(attackers, enemyBase.Point);

        Log.Warning($"Attacking with {attackers.Units.Count} {attackers.Units.FirstOrDefault()?.UnitType}");
    }
}