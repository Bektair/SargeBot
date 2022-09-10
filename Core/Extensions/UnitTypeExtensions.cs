namespace Core.Extensions;

public static class UnitTypeExtensions
{
    public static bool Is(this uint n, UnitType e)
    {
        return n == (uint)e;
    }

    // could be abstract and implemented in faction specific data service
    public static bool IsWorker(this uint u)
    {
        return (UnitType)u is UnitType.TERRAN_SCV or UnitType.PROTOSS_PROBE or UnitType.ZERG_DRONE;
    }

    // could be abstract and implemented in faction specific data service
    public static bool IsTownCenter(this uint unitType)
    {
        return (UnitType)unitType is UnitType.PROTOSS_NEXUS or
            UnitType.ZERG_HATCHERY or UnitType.ZERG_LAIR or UnitType.ZERG_HIVE or
            UnitType.TERRAN_COMMANDCENTER or UnitType.TERRAN_ORBITALCOMMAND or UnitType.TERRAN_PLANETARYFORTRESS or
            UnitType.TERRAN_COMMANDCENTERFLYING or UnitType.TERRAN_ORBITALCOMMANDFLYING;
    }

    /// <summary>
    ///     Minerals have different IDs depending on the game map (due to theme)
    /// </summary>
    /// <param name="id">UnitType ID as defined by Blizzard</param>
    /// <returns>Returns true if the ID is of the type mineral field.</returns>
    public static bool IsMineralField(this uint id)
    {
        return id is 146 or 147 or 341 or 483 or 885 or 884 or 665 or 666 or 795 or 797;
    }

    /// <summary>
    ///     Vespene Geysers have different IDs depending on the game map (due to theme)
    /// </summary>
    /// <param name="id">UnitType ID as defined by Blizzard</param>
    /// <returns>Returns true if the ID is of the type vespene geyser.</returns>
    public static bool IsVepeneGeyser(this uint id)
    {
        return id is 342 or 344 or 343 or 880 or 608 or 881;
    }

    public static bool IsVespeneCollectingBuilding(this uint id)
    {
      return id is 88 or 20 or 61;
    }


  /// <summary>
  ///     All IDs between 362 and 377 is (path) blocking and destructible.
  ///     IDs ranging from 472 - 474 is (placement) blocking and destructible.
  ///     IDs ranging from 623 - 658 is (placement) blocking and destructible.
  /// </summary>
  /// <param name="id">UnitType ID as defined by Blizzard</param>
  /// <returns>Returns true if the ID is of the type destructible and blocks pathing.</returns>
  public static bool IsDestructible(this uint id)
    {
        return id is > 361 and < 378 or >= 472 and <= 474 or >= 623 and <= 658;
    }

    /// <summary>
    ///     XelNagaTower (149)
    /// </summary>
    /// <param name="id">UnitType ID as defined by Blizzard</param>
    /// <returns>Returns true if the ID is of the type Xel Naga Tower.</returns>
    public static bool IsXelNagaTower(this uint id)
    {
        return id == 149;
    }
}