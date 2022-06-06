using Google.Protobuf.Collections;
using SC2APIProtocol;
using static SC2APIProtocol.AbilityData.Types;
using SC2ClientApi.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace SargeBot.Features.GameData;
public class GameDataService
{
    public Dictionary<Abilities,Ability> abilitiesDict { get; set; }
    public Dictionary<UnitTypes, Unit> unitsDict { get; set; } 
    public Dictionary<Upgrades, Upgrade> upgradesDict { get; set; }

    [JsonIgnore]
    public Dictionary<UnitTypes, UnitTypes> UnitToProducer = new Dictionary<UnitTypes, UnitTypes>()
    {   //Protoss
        { UnitTypes.PROTOSS_ZEALOT, UnitTypes.PROTOSS_GATEWAY },
        { UnitTypes.PROTOSS_STALKER, UnitTypes.PROTOSS_GATEWAY },
        { UnitTypes.PROTOSS_ADEPT, UnitTypes.PROTOSS_GATEWAY },
        { UnitTypes.PROTOSS_SENTRY, UnitTypes.PROTOSS_GATEWAY },
        { UnitTypes.PROTOSS_DARKSHRINE, UnitTypes.PROTOSS_GATEWAY },
        { UnitTypes.PROTOSS_HIGHTEMPLAR, UnitTypes.PROTOSS_GATEWAY },
        { UnitTypes.PROTOSS_IMMORTAL, UnitTypes.PROTOSS_ROBOTICSFACILITY },
        { UnitTypes.PROTOSS_OBSERVER, UnitTypes.PROTOSS_ROBOTICSFACILITY },
        { UnitTypes.PROTOSS_WARPPRISM, UnitTypes.PROTOSS_ROBOTICSFACILITY },
        { UnitTypes.PROTOSS_DISRUPTOR, UnitTypes.PROTOSS_ROBOTICSFACILITY },
        { UnitTypes.PROTOSS_COLOSSUS, UnitTypes.PROTOSS_ROBOTICSFACILITY },
        { UnitTypes.PROTOSS_VOIDRAY, UnitTypes.PROTOSS_STARGATE},
        { UnitTypes.PROTOSS_ORACLE, UnitTypes.PROTOSS_STARGATE},
        { UnitTypes.PROTOSS_PHOENIX, UnitTypes.PROTOSS_STARGATE},
        { UnitTypes.PROTOSS_CARRIER, UnitTypes.PROTOSS_STARGATE},
        { UnitTypes.PROTOSS_TEMPEST, UnitTypes.PROTOSS_STARGATE},
        { UnitTypes.PROTOSS_MOTHERSHIP, UnitTypes.PROTOSS_NEXUS},
        { UnitTypes.PROTOSS_PROBE, UnitTypes.PROTOSS_NEXUS},
        //Terran
        { UnitTypes.TERRAN_MARINE, UnitTypes.TERRAN_BARRACKS},
        { UnitTypes.TERRAN_MARAUDER, UnitTypes.TERRAN_BARRACKS},
        { UnitTypes.TERRAN_REAPER, UnitTypes.TERRAN_BARRACKS},
        { UnitTypes.TERRAN_GHOST, UnitTypes.TERRAN_BARRACKS},
        { UnitTypes.TERRAN_WIDOWMINE, UnitTypes.TERRAN_FACTORY},
        { UnitTypes.TERRAN_HELLION, UnitTypes.TERRAN_FACTORY},
        { UnitTypes.TERRAN_SIEGETANK, UnitTypes.TERRAN_FACTORY},
        { UnitTypes.TERRAN_THOR, UnitTypes.TERRAN_FACTORY},
        { UnitTypes.TERRAN_CYCLONE, UnitTypes.TERRAN_FACTORY},
        { UnitTypes.TERRAN_HELLIONTANK, UnitTypes.TERRAN_FACTORY},
        { UnitTypes.TERRAN_VIKINGASSAULT, UnitTypes.TERRAN_STARPORT},
        { UnitTypes.TERRAN_LIBERATOR, UnitTypes.TERRAN_STARPORT},
        { UnitTypes.TERRAN_RAVEN, UnitTypes.TERRAN_STARPORT},
        { UnitTypes.TERRAN_BATTLECRUISER, UnitTypes.TERRAN_STARPORT},
        { UnitTypes.TERRAN_BANSHEE, UnitTypes.TERRAN_STARPORT},
        { UnitTypes.TERRAN_SCV, UnitTypes.TERRAN_COMMANDCENTER},
        //Zergy?

    };

    




    public GameDataService()
    {
        abilitiesDict = new Dictionary<Abilities, Ability>();
        unitsDict = new Dictionary<UnitTypes, Unit>();
        upgradesDict = new Dictionary<Upgrades, Upgrade>();
    }

    public void FillAbilities(RepeatedField<AbilityData> apiAbilities)
    {
        foreach (AbilityData abilityData in apiAbilities)
        {
            if(abilityData.FriendlyName != String.Empty) //removes dummies like index 0
                abilitiesDict.Add((Abilities)abilityData.AbilityId, new Ability(abilityData));
        }
    }

    //Helper to fix broken part of API
    private List<UnitTypes> requireAttached = new()
    {
        UnitTypes.TERRAN_MARAUDER,
        UnitTypes.TERRAN_GHOST,
        UnitTypes.TERRAN_SIEGETANK,
        UnitTypes.TERRAN_CYCLONE,
        UnitTypes.TERRAN_THOR,
        UnitTypes.TERRAN_BATTLECRUISER,
        UnitTypes.TERRAN_RAVEN,
        UnitTypes.TERRAN_LIBERATOR,
        UnitTypes.TERRAN_BANSHEE,
    };

    private Dictionary<UnitTypes, UnitTypes> techRequirementTerran = new()
    {
        { UnitTypes.TERRAN_GHOST, UnitTypes.TERRAN_GHOSTACADEMY},
        { UnitTypes.TERRAN_HELLIONTANK, UnitTypes.TERRAN_ARMORY },
        { UnitTypes.TERRAN_THOR, UnitTypes.TERRAN_ARMORY },
        { UnitTypes.TERRAN_BATTLECRUISER, UnitTypes.TERRAN_FUSIONCORE },
    };

    public void FillUnits(RepeatedField<UnitTypeData> unitTypeDatas)
    {
        foreach(UnitTypes unit in requireAttached) //Fixx for terran
            unitTypeDatas.ElementAt((int)unit).RequireAttached = true;
        foreach (UnitTypes unit in techRequirementTerran.Keys) //Fixx for terran
            unitTypeDatas.ElementAt((int)unit).TechRequirement = (uint)techRequirementTerran.GetValueOrDefault(unit);


        foreach (UnitTypeData unitData in unitTypeDatas)
        {
            if(unitData.Name != String.Empty) //removes dummies like index 0
                unitsDict.Add((UnitTypes)unitData.UnitId, new Unit(unitData));
        }
    }

    public void FillUpgrades(RepeatedField<UpgradeData> upgradeData)
    {
        foreach (UpgradeData upgrade in upgradeData)
        {
            if (upgrade.Name!=String.Empty) //removes dummies like index 0
                upgradesDict.Add((Upgrades)upgrade.UpgradeId, new Upgrade(upgrade));
        }
    }

    public void FillAllData(ResponseData data)
    {
        FillAbilities(data.Abilities);
        FillUnits(data.Units);
        FillUpgrades(data.Upgrades);
    }


}

