using SC2APIProtocol;
using SC2ClientApi;

namespace Core.Protoss;

public class ProbeRushBot : ProtossBot
{
    public ProbeRushBot(IServiceProvider services) : base(services)
    {
    }

    public override void OnFrame(ResponseObservation observation)
    {
        base.OnFrame(observation);

        UnitService.Train(UnitType.PROTOSS_PROBE);

        if (Intel.GetWorkers().Count < 14) return;

        var enemyBase = Intel.EnemyColonies.First();
        
        Log.Warning($"Attacking with {Intel.GetWorkers().Count} {nameof(UnitType.PROTOSS_PROBE)}");

        var workers = new Squad();
        workers.AddUnits(Intel.GetWorkers());

        MicroService.AttackMove(workers, enemyBase.Point);
    }
}