﻿using SC2APIProtocol;
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

        if (Intel.GetUnits(UnitType.ZERG_DRONE).Count < 14) return;

        var enemyBase = Intel.EnemyColonies.First();

        var attackers = new Squad();
        attackers.AddUnits(Intel.GetUnits(UnitType.ZERG_DRONE));

        MicroService.AttackMove(attackers, enemyBase.Point);

        Log.Warning($"Attacking with {attackers.Units.Count} {attackers.Units.FirstOrDefault()?.UnitType}");

        var overlords = new Squad();
        overlords.AddUnits(Intel.GetUnits(UnitType.ZERG_OVERLORD));

        MicroService.Move(overlords, enemyBase.Point);
    }
}