using SC2ClientApi.Constants;

namespace SC2ClientApi;

public static class UnitTypeHelpers
{
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

    public static bool IsMainBuilding(this uint unitType)
    {
        return (UnitTypes) unitType is UnitTypes.PROTOSS_NEXUS or
            UnitTypes.ZERG_HATCHERY or UnitTypes.ZERG_LAIR or UnitTypes.ZERG_HIVE or
            UnitTypes.TERRAN_COMMANDCENTER or UnitTypes.TERRAN_COMMANDCENTERFLYING or UnitTypes.TERRAN_ORBITALCOMMAND or UnitTypes.TERRAN_COMMANDCENTERFLYING or UnitTypes.TERRAN_PLANETARYFORTRESS;
    }
}