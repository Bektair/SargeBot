namespace Core.Terran;

public static class TerranDataHelpers
{
    //TODO: more producers
    public static readonly Dictionary<UnitType, (UnitType Type, Ability Ability)[]> Producers = new()
    {
        {
            UnitType.TERRAN_SCV,
            new[]
            {
                (UnitType.TERRAN_COMMANDCENTER, Ability.TRAIN_SCV),
                (UnitType.TERRAN_ORBITALCOMMAND, Ability.TRAIN_SCV),
                (UnitType.TERRAN_PLANETARYFORTRESS, Ability.TRAIN_SCV)
            }
        },
        { UnitType.TERRAN_MARINE, new[] { (UnitType.TERRAN_BARRACKS, Ability.TRAIN_MARINE) } },
        
        { UnitType.TERRAN_COMMANDCENTER, new[] { (UnitType.TERRAN_SCV, Ability.BUILD_COMMANDCENTER) } },
        { UnitType.TERRAN_SUPPLYDEPOT, new[] { (UnitType.TERRAN_SCV, Ability.BUILD_SUPPLYDEPOT) } },
        { UnitType.TERRAN_BARRACKS, new[] { (UnitType.TERRAN_SCV, Ability.BUILD_BARRACKS) } },
    };

    public static readonly List<UnitType> RequiresTechLab = new()
    {
        UnitType.TERRAN_MARAUDER,
        UnitType.TERRAN_GHOST,
        UnitType.TERRAN_SIEGETANK,
        UnitType.TERRAN_CYCLONE,
        UnitType.TERRAN_THOR,
        UnitType.TERRAN_BATTLECRUISER,
        UnitType.TERRAN_RAVEN,
        UnitType.TERRAN_LIBERATOR,
        UnitType.TERRAN_BANSHEE
    };

    public static readonly Dictionary<UnitType, UnitType> TechRequirement = new()
    {
        { UnitType.TERRAN_GHOST, UnitType.TERRAN_GHOSTACADEMY },
        { UnitType.TERRAN_HELLIONTANK, UnitType.TERRAN_ARMORY },
        { UnitType.TERRAN_THOR, UnitType.TERRAN_ARMORY },
        { UnitType.TERRAN_BATTLECRUISER, UnitType.TERRAN_FUSIONCORE }
    };
}