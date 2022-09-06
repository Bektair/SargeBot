using SC2APIProtocol;
using SC2ClientApi;

namespace Core.Zerg;

public class DroneRushBot : ZergBot
{
    public DroneRushBot(IServiceProvider services) : base(services)
    {
    }

    public override void OnFrame(ResponseObservation observation)
    {
        base.OnFrame(observation);

        MacroService.Train(UnitType.ZERG_DRONE);

        if (Intel.GetWorkers().Count < 14) return;

        var enemyBase = Intel.EnemyColonies.First();

        Log.Warning($"Attacking with {Intel.GetWorkers().Count} {nameof(UnitType.ZERG_DRONE)}");

        var workers = new Squad();
        workers.AddUnits(Intel.GetWorkers());

        MicroService.AttackMove(workers, enemyBase.Point);

        var overlords = new Squad();
        overlords.AddUnits(OverlordService.GetOverlords());

        MicroService.Move(overlords, enemyBase.Point);
    }
}