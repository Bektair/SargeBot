namespace Core.Terran;

public static class ProtossDataHelpers
{
    //TODO: more producers
    public static readonly Dictionary<UnitType, (UnitType Type, Ability Ability)[]> Producers = new()
    {
        { UnitType.PROTOSS_PROBE, new[] { (UnitType.PROTOSS_NEXUS, Ability.TRAIN_PROBE) } },
        { UnitType.PROTOSS_NEXUS, new[] { (UnitType.PROTOSS_PROBE, Ability.BUILD_NEXUS) } }
    };
}