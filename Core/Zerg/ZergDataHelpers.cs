namespace Core.Zerg;

public static class ZergDataHelpers
{
    //TODO: more producers
    public static readonly Dictionary<UnitType, (UnitType Type, Ability Ability)[]> Producers = new()
    {
        { UnitType.ZERG_DRONE, new[] { (UnitType.ZERG_LARVA, Ability.TRAIN_DRONE) } },
        { UnitType.ZERG_ZERGLING, new[] { (UnitType.ZERG_LARVA, Ability.TRAIN_ZERGLING) } },
        { UnitType.ZERG_EXTRACTOR, new[] { (UnitType.ZERG_DRONE, Ability.BUILD_EXTRACTOR) } },
    };
}