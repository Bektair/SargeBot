using Google.Protobuf.Collections;
using SC2APIProtocol;
using static SC2APIProtocol.AbilityData.Types;
using SC2ClientApi.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.GameData;
public class GameData
{
    Dictionary<Abilities,Ability> abilitiesDict = new Dictionary<Abilities, Ability>();
    Dictionary<UnitTypes, Unit> unitsDict = new Dictionary<UnitTypes, Unit>();
    Dictionary<Upgrades, Upgrade> upgradesDict = new Dictionary<Upgrades, Upgrade>();


    public GameData()
    {
    }
    public void FillAbilities(RepeatedField<AbilityData> apiAbilities)
    {
        foreach(AbilityData abilityData in apiAbilities)
        {
            if(abilityData.HasAbilityId)
                abilitiesDict.Add((Abilities)abilityData.AbilityId, new Ability(abilityData));
        }

    }

    public void FillUnits(RepeatedField<UnitTypeData> unitTypeDatas)
    {
        foreach (UnitTypeData unitData in unitTypeDatas)
        {
            if(unitData.HasUnitId)
                unitsDict.Add((UnitTypes)unitData.UnitId, new Unit(unitData));
        }
    }

    public void FillUpgrades(RepeatedField<UpgradeData> upgradeData)
    {
        foreach (UpgradeData upgrade in upgradeData)
        {
            if (upgrade.HasUpgradeId)
                upgradesDict.Add((Upgrades)upgrade.UpgradeId, new Upgrade(upgrade));
        }
    }


}

