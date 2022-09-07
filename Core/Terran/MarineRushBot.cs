using SC2APIProtocol;
using SC2ClientApi;

namespace Core.Terran;

public class MarineRushBot : TerranBot
{
    public MarineRushBot(IServiceProvider services) : base(services)
    {
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

        if (Intel.GetUnits(UnitType.TERRAN_MARINE).Count < 5)
            return;

        var enemyBase = Intel.EnemyColonies.First();

        var attackers = new Squad();
        attackers.AddUnits(Intel.GetUnits(UnitType.TERRAN_MARINE));

        MicroService.AttackMove(attackers, enemyBase.Point);

        Log.Warning($"Attacking with {attackers.Units.Count} {attackers.Units.FirstOrDefault()?.UnitType}");
    }
}