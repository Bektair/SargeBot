using Core.Bot;
using Core.Debug;
using SC2APIProtocol;

namespace BillyBot;

public class DebugBot : BaseBot
{
    public DebugBot(IServiceProvider services) : base(services, Race.Terran)
    {
    }

    public override void OnFrame(ResponseObservation observation)
    {
        base.OnFrame(observation);

        if (observation.Observation.GameLoop % 1000 == 0)
            MessageService.Debug(DebugRequest.DrawText($"Frame {observation.Observation.GameLoop}"));

        var z = 12;

        var enemyBase = Intel.EnemyColonies.First();
        MessageService.Debug(DebugRequest.DrawBox(new Point { X = enemyBase.Point.X, Y = enemyBase.Point.Y, Z = z },
            new Point { X = enemyBase.Point.X + 5, Y = enemyBase.Point.Y + 5, Z = z }, new Color { B = 255 }));

        var firstWorker = Intel.GetWorkers().First();
        MessageService.Debug(DebugRequest.DrawSphere(new Point { X = firstWorker.Point.X, Y = firstWorker.Point.Y, Z = z },
            color: new Color { G = 255 }));
    }
}