using SC2APIProtocol;
using SC2ClientApi;

namespace Core.Zerg;

public class DroneRushBot : ZergBot
{
    public DroneRushBot(IServiceProvider services) : base(services)
    {
    }

    protected override void OnFrame(ResponseObservation observation)
    {
        base.OnFrame(observation);

        MacroService.Train(UnitType.ZERG_DRONE);

        if (Intel.GetWorkers().Count < 14) return;

        var enemyBase = Intel.EnemyColonies.First();

        var attackers = new Squad();
        attackers.AddUnits(Intel.GetWorkers());

        MicroService.AttackMove(attackers, enemyBase.Point);

        Log.Warning($"Attacking with {attackers.Units.Count} {attackers.Units.FirstOrDefault()?.UnitType}");

        var overlords = new Squad();
        overlords.AddUnits(OverlordService.GetOverlords());

        MicroService.Move(overlords, enemyBase.Point);
    }
}