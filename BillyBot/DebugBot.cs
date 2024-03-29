﻿using Core;
using SC2APIProtocol;

namespace BillyBot;

public class DebugBot : BaseBot
{
    public DebugBot(IServiceProvider services) : base(services, Race.Terran)
    {
    }

    protected override void OnFrame(ResponseObservation observation)
    {
        base.OnFrame(observation);

        var z = 12;
        var mainBase = Intel.Colonies.First();
        var enemyBase = Intel.EnemyColonies.First();
        var firstGas = Intel.GetVespeneGeysers().First();
        var firstWorker = Intel.GetUnits(UnitType.TERRAN_SCV).First();
        var lastWorker = Intel.GetUnits(UnitType.TERRAN_SCV).Last();

        // Color requires all three values
        var color = new Color { G = 0, R = 250, B = 50 };

        // Text - fixed on screen
        MessageService.Debug(DebugRequest.DrawText($"Worker count {Intel.GetUnits(UnitType.TERRAN_SCV).Count}", x: .5f, color: color));

        // Text - fixed on map
        MessageService.Debug(DebugRequest.DrawText("Main base", x: mainBase.Point.X, y: mainBase.Point.Y, color: color, worldPosition: true));

        // Box - min: bottom left, max: top right
        MessageService.Debug(DebugRequest.DrawBox(
            new Point { X = enemyBase.Point.X + 2, Y = enemyBase.Point.Y + 2, Z = z },
            new Point { X = enemyBase.Point.X + 5, Y = enemyBase.Point.Y + 5, Z = z },
            color));

        // Sphere 
        MessageService.Debug(DebugRequest.DrawSphere(
            new Point { X = firstGas.Point.X, Y = firstGas.Point.Y, Z = z },
            color: color));

        // Line
        MessageService.Debug(DebugRequest.DrawLine(firstWorker.Pos, lastWorker.Pos, color));
    }
}