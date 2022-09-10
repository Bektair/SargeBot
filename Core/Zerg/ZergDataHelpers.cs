namespace Core.Zerg;

public static class ZergDataHelpers
{
  public static readonly Dictionary<UnitType, (UnitType Type, Ability Ability)[]> Producers = new()
  {
    //Units
    { UnitType.ZERG_DRONE, new[] { (UnitType.ZERG_LARVA, Ability.TRAIN_DRONE) } },
    { UnitType.ZERG_ZERGLING, new[] { (UnitType.ZERG_LARVA, Ability.TRAIN_ZERGLING) } },
    { UnitType.ZERG_ROACH, new[] { (UnitType.ZERG_LARVA, Ability.TRAIN_ROACH) } },
    { UnitType.ZERG_HYDRALISK, new[] { (UnitType.ZERG_LARVA, Ability.TRAIN_HYDRALISK) } },
    { UnitType.ZERG_OVERLORD, new[] { (UnitType.ZERG_LARVA, Ability.TRAIN_OVERLORD) } },
    { UnitType.ZERG_INFESTOR, new[] { (UnitType.ZERG_LARVA, Ability.TRAIN_INFESTOR) } },
    { UnitType.ZERG_MUTALISK, new[] { (UnitType.ZERG_LARVA, Ability.TRAIN_MUTALISK) } },
    { UnitType.ZERG_CORRUPTOR, new[] { (UnitType.ZERG_LARVA, Ability.TRAIN_CORRUPTOR) } },
    { UnitType.ZERG_ULTRALISK, new[] { (UnitType.ZERG_LARVA, Ability.TRAIN_ULTRALISK) } },
    { UnitType.ZERG_SWARMHOSTMP, new[] { (UnitType.ZERG_LARVA, Ability.TRAIN_SWARMHOST) } },
    { UnitType.ZERG_VIPER, new[] { (UnitType.ZERG_LARVA, Ability.TRAIN_VIPER) } },
    { UnitType.ZERG_QUEEN, new[] { (UnitType.ZERG_HATCHERY, Ability.TRAIN_QUEEN), (UnitType.ZERG_HIVE, Ability.TRAIN_QUEEN), (UnitType.ZERG_LAIR, Ability.TRAIN_QUEEN) } },
    //Buildings
    { UnitType.ZERG_EXTRACTOR, new[] { (UnitType.ZERG_DRONE, Ability.BUILD_EXTRACTOR) } },
    { UnitType.ZERG_SPAWNINGPOOL, new[] { (UnitType.ZERG_DRONE, Ability.BUILD_SPAWNINGPOOL) } },
    { UnitType.ZERG_HATCHERY, new[] { (UnitType.ZERG_DRONE, Ability.BUILD_HATCHERY) } },
    { UnitType.ZERG_ROACHWARREN, new[] { (UnitType.ZERG_DRONE, Ability.BUILD_ROACHWARREN) } },
    { UnitType.ZERG_BANELINGNEST, new[] { (UnitType.ZERG_DRONE, Ability.BUILD_BANELINGNEST) } },
    { UnitType.ZERG_HYDRALISKDEN, new[] { (UnitType.ZERG_DRONE, Ability.BUILD_HYDRALISKDEN) } },
    { UnitType.ZERG_SPORECRAWLER, new[] { (UnitType.ZERG_DRONE, Ability.BUILD_SPORECRAWLER) } },
    { UnitType.ZERG_SPINECRAWLER, new[] { (UnitType.ZERG_DRONE, Ability.BUILD_SPINECRAWLER) } },
    { UnitType.ZERG_EVOLUTIONCHAMBER, new[] { (UnitType.ZERG_DRONE, Ability.BUILD_EVOLUTIONCHAMBER) } },
    { UnitType.ZERG_INFESTATIONPIT, new[] { (UnitType.ZERG_DRONE, Ability.BUILD_INFESTATIONPIT) } },
    { UnitType.ZERG_SPIRE, new[] { (UnitType.ZERG_DRONE, Ability.BUILD_SPIRE) } },
    { UnitType.ZERG_ULTRALISKCAVERN, new[] { (UnitType.ZERG_DRONE, Ability.BUILD_ULTRALISKCAVERN) } },
    { UnitType.ZERG_NYDUSNETWORK, new[] { (UnitType.ZERG_DRONE, Ability.BUILD_NYDUSNETWORK) } },
    //Morphs
    { UnitType.ZERG_GREATERSPIRE, new[] { (UnitType.ZERG_SPIRE, Ability.MORPH_GREATERSPIRE) } },
    { UnitType.ZERG_LURKERDENMP, new[] { (UnitType.ZERG_HYDRALISKDEN, Ability.Morph_LurkerDen) } },
    { UnitType.ZERG_LAIR, new[] { (UnitType.ZERG_HATCHERY, Ability.MORPH_LAIR) } },
    { UnitType.ZERG_HIVE, new[] { (UnitType.ZERG_LAIR, Ability.MORPH_HIVE) } },
    { UnitType.ZERG_BANELING, new[] { (UnitType.ZERG_ZERGLING, Ability.MORPH_BANELING) } },
    { UnitType.ZERG_BROODLORD, new[] { (UnitType.ZERG_CORRUPTOR, Ability.MORPH_BROODLORD) } },
    { UnitType.ZERG_RAVAGER, new[] { (UnitType.ZERG_ROACH, Ability.MORPH_RAVAGER) } },
    { UnitType.ZERG_LURKERMP, new[] { (UnitType.ZERG_HYDRALISK, Ability.Morph_Lurker) } },
    { UnitType.ZERG_OVERSEER, new[] { (UnitType.ZERG_OVERLORD, Ability.Morph_Overseer) } },
    

  };
}