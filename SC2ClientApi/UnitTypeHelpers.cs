using SC2ClientApi.Constants;

namespace SC2ClientApi;

public static class UnitTypeHelpers
{
    public static bool Is(this uint n, UnitType e) => n == (uint) e;
    public static bool Is(this uint n, UnitTypes e) => n == (uint) e;

    public static bool IsDestructible(this uint unitType)
    {
        return (UnitTypes) unitType switch
        {
            UnitTypes.NEUTRAL_DESTRUCTIBLEROCK6X6 => true,
            UnitTypes.NEUTRAL_DESTRUCTIBLEDEBRIS6X6 => true,
            UnitTypes.NEUTRAL_DESTRUCTIBLEROCKEX1DIAGONALHUGEBLUR => true,
            UnitTypes.NEUTRAL_DESTRUCTIBLEDEBRISRAMPDIAGONALHUGEBLUR => true,
            UnitTypes.NEUTRAL_UNBUILDABLEBRICKSDESTRUCTIBLE => true,
            UnitTypes.NEUTRAL_UNBUILDABLEPLATESDESTRUCTIBLE => true,
            _ => false
        };
    }

    public static bool IsMainBuilding(this uint unitType) =>
        (UnitTypes) unitType is UnitTypes.PROTOSS_NEXUS or
        UnitTypes.ZERG_HATCHERY or UnitTypes.ZERG_LAIR or UnitTypes.ZERG_HIVE or
        UnitTypes.TERRAN_COMMANDCENTER or UnitTypes.TERRAN_COMMANDCENTERFLYING or UnitTypes.TERRAN_ORBITALCOMMAND or UnitTypes.TERRAN_COMMANDCENTERFLYING or UnitTypes.TERRAN_PLANETARYFORTRESS;

    /// <summary>
    ///     Minerals have different IDs depending on the game map (due to theme)
    /// </summary>
    /// <param name="id">UnitType ID as defined by Blizzard</param>
    /// <returns>Returns true if the ID is of the type mineral field.</returns>
    public static bool IsMineralField(this uint id) => id is 146 or 147 or 341 or 483 or 885 or 884 or 665 or 666;
}