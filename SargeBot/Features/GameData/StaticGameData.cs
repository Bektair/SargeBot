using System.Text.Json;
using System.Text.Json.Serialization;
using Google.Protobuf.Collections;
using SC2APIProtocol;

namespace SargeBot.Features.GameData;

public class StaticGameData
{
    // Helpers to fix broken part of API
    private readonly List<UnitType> RequireAttached = new()
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

    private readonly Dictionary<UnitType, UnitType> TechRequirementTerran = new()
    {
        {UnitType.TERRAN_GHOST, UnitType.TERRAN_GHOSTACADEMY},
        {UnitType.TERRAN_HELLIONTANK, UnitType.TERRAN_ARMORY},
        {UnitType.TERRAN_THOR, UnitType.TERRAN_ARMORY},
        {UnitType.TERRAN_BATTLECRUISER, UnitType.TERRAN_FUSIONCORE}
    };

    #region UnitToProducer

    public Dictionary<UnitType, UnitType> UnitToProducer = new()
    {
        // Protoss
        {UnitType.PROTOSS_ZEALOT, UnitType.PROTOSS_GATEWAY},
        {UnitType.PROTOSS_STALKER, UnitType.PROTOSS_GATEWAY},
        {UnitType.PROTOSS_ADEPT, UnitType.PROTOSS_GATEWAY},
        {UnitType.PROTOSS_SENTRY, UnitType.PROTOSS_GATEWAY},
        {UnitType.PROTOSS_DARKSHRINE, UnitType.PROTOSS_GATEWAY},
        {UnitType.PROTOSS_HIGHTEMPLAR, UnitType.PROTOSS_GATEWAY},
        {UnitType.PROTOSS_IMMORTAL, UnitType.PROTOSS_ROBOTICSFACILITY},
        {UnitType.PROTOSS_OBSERVER, UnitType.PROTOSS_ROBOTICSFACILITY},
        {UnitType.PROTOSS_WARPPRISM, UnitType.PROTOSS_ROBOTICSFACILITY},
        {UnitType.PROTOSS_DISRUPTOR, UnitType.PROTOSS_ROBOTICSFACILITY},
        {UnitType.PROTOSS_COLOSSUS, UnitType.PROTOSS_ROBOTICSFACILITY},
        {UnitType.PROTOSS_VOIDRAY, UnitType.PROTOSS_STARGATE},
        {UnitType.PROTOSS_ORACLE, UnitType.PROTOSS_STARGATE},
        {UnitType.PROTOSS_PHOENIX, UnitType.PROTOSS_STARGATE},
        {UnitType.PROTOSS_CARRIER, UnitType.PROTOSS_STARGATE},
        {UnitType.PROTOSS_TEMPEST, UnitType.PROTOSS_STARGATE},
        {UnitType.PROTOSS_MOTHERSHIP, UnitType.PROTOSS_NEXUS},
        {UnitType.PROTOSS_PROBE, UnitType.PROTOSS_NEXUS},
        // Terran
        {UnitType.TERRAN_MARINE, UnitType.TERRAN_BARRACKS},
        {UnitType.TERRAN_MARAUDER, UnitType.TERRAN_BARRACKS},
        {UnitType.TERRAN_REAPER, UnitType.TERRAN_BARRACKS},
        {UnitType.TERRAN_GHOST, UnitType.TERRAN_BARRACKS},
        {UnitType.TERRAN_WIDOWMINE, UnitType.TERRAN_FACTORY},
        {UnitType.TERRAN_HELLION, UnitType.TERRAN_FACTORY},
        {UnitType.TERRAN_SIEGETANK, UnitType.TERRAN_FACTORY},
        {UnitType.TERRAN_THOR, UnitType.TERRAN_FACTORY},
        {UnitType.TERRAN_CYCLONE, UnitType.TERRAN_FACTORY},
        {UnitType.TERRAN_HELLIONTANK, UnitType.TERRAN_FACTORY},
        {UnitType.TERRAN_VIKINGASSAULT, UnitType.TERRAN_STARPORT},
        {UnitType.TERRAN_LIBERATOR, UnitType.TERRAN_STARPORT},
        {UnitType.TERRAN_RAVEN, UnitType.TERRAN_STARPORT},
        {UnitType.TERRAN_BATTLECRUISER, UnitType.TERRAN_STARPORT},
        {UnitType.TERRAN_BANSHEE, UnitType.TERRAN_STARPORT},
        {UnitType.TERRAN_SCV, UnitType.TERRAN_COMMANDCENTER},
        // Zerg
        {UnitType.ZERG_ZERGLING, UnitType.ZERG_LARVA},
        {UnitType.ZERG_ROACH, UnitType.ZERG_LARVA},
        {UnitType.ZERG_HYDRALISK, UnitType.ZERG_LARVA},
        {UnitType.ZERG_SWARMHOSTMP, UnitType.ZERG_LARVA},
        {UnitType.ZERG_OVERLORD, UnitType.ZERG_LARVA},
        {UnitType.ZERG_VIPER, UnitType.ZERG_LARVA},
        {UnitType.ZERG_DRONE, UnitType.ZERG_LARVA},
        {UnitType.ZERG_CORRUPTOR, UnitType.ZERG_LARVA},
        {UnitType.ZERG_MUTALISK, UnitType.ZERG_LARVA},
        {UnitType.ZERG_ULTRALISK, UnitType.ZERG_LARVA},
        {UnitType.ZERG_INFESTOR, UnitType.ZERG_LARVA},
        {UnitType.ZERG_QUEEN, UnitType.ZERG_HATCHERY}
    };

  #endregion

  public List<UnitType> hatcheryLike = new() { UnitType.ZERG_HATCHERY, UnitType.ZERG_LAIR, UnitType.ZERG_HIVE };
  public List<UnitType> gatewayLike = new() { UnitType.PROTOSS_GATEWAY, UnitType.PROTOSS_WARPGATE };


    public Dictionary<Ability, PlainAbility> PlainAbilities { get; set; } = new();
    public Dictionary<UnitType, PlainUnit> PlainUnits { get; set; } = new();
    public Dictionary<Upgrade, PlainUpgrade> PlainUpgrades { get; set; } = new();

    public void PopulateGameData(ResponseData responseData)
    {
        PopulateAbilities(responseData.Abilities);
        PopulateUnits(responseData.Units);
        PopulateUpgrades(responseData.Upgrades);
    }

    private void PopulateAbilities(RepeatedField<AbilityData> abilityDatas)
    {
        foreach (var abilityData in abilityDatas)
        {
            if (abilityData.FriendlyName == "") continue;
            PlainAbilities.Add((Ability) abilityData.AbilityId, new(abilityData));
        }
    }

    private void PopulateUnits(RepeatedField<UnitTypeData> unitTypeDatas)
    {
        foreach (var unitTypeData in unitTypeDatas)
        {
            if (TechRequirementTerran.TryGetValue((UnitType) unitTypeData.UnitId, out var techReq))
                unitTypeData.TechRequirement = (uint) techReq;

            if (RequireAttached.Contains((UnitType) unitTypeData.UnitId))
                unitTypeData.RequireAttached = true;

            PlainUnits.Add((UnitType) unitTypeData.UnitId, new(unitTypeData));
        }
    }

    private void PopulateUpgrades(RepeatedField<UpgradeData> upgradesDatas)
    {
        foreach (var upgradeData in upgradesDatas)
        {
            if (upgradeData.Name == "") continue;
            PlainUpgrades.Add((Upgrade) upgradeData.UpgradeId, new(upgradeData));
        }
    }

    public async Task Save()
    {
        await SaveToFile(PlainAbilities, "plain-abilities.json");
        await SaveToFile(PlainUnits, "plain-units.json");
        await SaveToFile(PlainUpgrades, "plain-upgrades.json");
    }

    private static async Task SaveToFile<TKey, TValue>(Dictionary<TKey, TValue> gameData, string fileName) where TKey : notnull
    {
        if (!Directory.Exists("data")) Directory.CreateDirectory("data");
        
        var filePath = Path.Combine("data", fileName);
        if (File.Exists(filePath)) return;

        var json = JsonSerializer.Serialize(gameData, new JsonSerializerOptions {WriteIndented = true, Converters = {new JsonStringEnumConverter()}});
        await File.WriteAllTextAsync(filePath, json);
        Console.WriteLine($"Saved {fileName}");
    }
}