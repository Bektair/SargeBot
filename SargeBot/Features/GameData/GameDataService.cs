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
    public Dictionary<uint,Ability> abilitiesDict { get; set; }
    public Dictionary<uint, Unit> unitsDict { get; set; } 
    public Dictionary<uint, Upgrade> upgradesDict { get; set; } 

   
    public GameDataService()
    {
        abilitiesDict = new Dictionary<uint, Ability>();
        unitsDict = new Dictionary<uint, Unit>();
        upgradesDict = new Dictionary<uint, Upgrade>();
    }


    public void FillAbilities(RepeatedField<AbilityData> apiAbilities)
    {
        foreach(AbilityData abilityData in apiAbilities)
        {
            if(abilityData.FriendlyName != String.Empty) //removes dummies like index 0
                abilitiesDict.Add(abilityData.AbilityId, new Ability(abilityData));
        }

    }

    public void FillUnits(RepeatedField<UnitTypeData> unitTypeDatas)
    {
        foreach (UnitTypeData unitData in unitTypeDatas)
        {
            if(unitData.Name != String.Empty) //removes dummies like index 0
                unitsDict.Add(unitData.UnitId, new Unit(unitData));
        }
    }

    public void FillUpgrades(RepeatedField<UpgradeData> upgradeData)
    {
        foreach (UpgradeData upgrade in upgradeData)
        {
            if (upgrade.Name!=String.Empty) //removes dummies like index 0
                upgradesDict.Add(upgrade.UpgradeId, new Upgrade(upgrade));
        }
    }


}

