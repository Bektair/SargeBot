using SC2APIProtocol;
using SC2ClientApi;

namespace Core.Terran;

public class MarineRushBot : TerranBot
{
    private bool _attacking;

    public MarineRushBot(IServiceProvider services) : base(services)
    {
    }

    public override void OnStart(ResponseObservation firstObservation, ResponseData responseData, ResponseGameInfo gameInfo)
    {
        base.OnStart(firstObservation, responseData, gameInfo);

        MessageService.Chat("GL HF");
    }

    public override void OnFrame(ResponseObservation observation)
    {
        base.OnFrame(observation);

        MacroService.Train(UnitType.TERRAN_SCV);

        if (Intel.GetStructures(UnitType.TERRAN_SUPPLYDEPOT).Count <= Intel.GetStructures(UnitType.TERRAN_BARRACKS).Count)
            MacroService.Build(UnitType.TERRAN_SUPPLYDEPOT);

        if (Intel.GetStructures(UnitType.TERRAN_BARRACKS).Count < 4)
            MacroService.Build(UnitType.TERRAN_BARRACKS);

        MacroService.Train(UnitType.TERRAN_MARINE);

        if (Intel.GetUnits(UnitType.TERRAN_MARINE).Count <= 10)
        {
            if (_attacking)
                Log.Warning("Stopped attacking");
            _attacking = false;
            return;
        }

        var enemyBase = Intel.EnemyColonies.First();

        var attackers = new Squad();
        attackers.AddUnits(Intel.GetUnits(UnitType.TERRAN_MARINE));

        MicroService.AttackMove(attackers, enemyBase.Point);

        if (!_attacking || Intel.GameLoop % 500 == 0)
            Log.Warning($"Attacking with {attackers.Units.Count} {(UnitType?)attackers.Units.FirstOrDefault()?.UnitType}");
        _attacking = true;
    }
}