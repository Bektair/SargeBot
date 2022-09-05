namespace Core.Terran;

public class TerranDataHelpers
{
    public static Dictionary<UnitType, (UnitType Type, Ability Ability)[]> _producers = new()
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
        { UnitType.TERRAN_COMMANDCENTER, new[] { (UnitType.TERRAN_SCV, Ability.BUILD_COMMANDCENTER) } }
    };

    private readonly List<UnitType> _requiresTechLab = new()
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

    private readonly Dictionary<UnitType, UnitType> _techRequirement = new()
    {
        { UnitType.TERRAN_GHOST, UnitType.TERRAN_GHOSTACADEMY },
        { UnitType.TERRAN_HELLIONTANK, UnitType.TERRAN_ARMORY },
        { UnitType.TERRAN_THOR, UnitType.TERRAN_ARMORY },
        { UnitType.TERRAN_BATTLECRUISER, UnitType.TERRAN_FUSIONCORE }
    };
}