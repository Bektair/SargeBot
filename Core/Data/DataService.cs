using SC2APIProtocol;
using Attribute = SC2APIProtocol.Attribute;

namespace Core.Data;

public class DataService : IDataService
{
    private readonly Dictionary<uint, AbilityData> _abilityData = new();
    private readonly Dictionary<uint, BuffData> _buffData = new();
    private readonly Dictionary<uint, UnitTypeData> _unitTypeData = new();
    private readonly Dictionary<uint, UpgradeData> upgradeData = new();
    
    // move to Terran specific data service
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
    
    // move to Terran specific data service
    private readonly Dictionary<UnitType, UnitType> _techRequirementTerran = new()
    {
        {UnitType.TERRAN_GHOST, UnitType.TERRAN_GHOSTACADEMY},
        {UnitType.TERRAN_HELLIONTANK, UnitType.TERRAN_ARMORY},
        {UnitType.TERRAN_THOR, UnitType.TERRAN_ARMORY},
        {UnitType.TERRAN_BATTLECRUISER, UnitType.TERRAN_FUSIONCORE}
    };

    public void OnStart(ResponseObservation obs, ResponseData? data, ResponseGameInfo? gameInfo)
    {
        if (data == null) throw new Exception("Invalid data");
        
        foreach (var ability in data.Abilities)
            _abilityData.Add(ability.AbilityId, ability);
        foreach (var buff in data.Buffs)
            _buffData.Add(buff.BuffId, buff);
        foreach (var unitType in data.Units)
            _unitTypeData.Add(unitType.UnitId, unitType);
        foreach (var upgrade in data.Upgrades)
            upgradeData.Add(upgrade.UpgradeId, upgrade);
    }
    
    public bool IsStructure(uint unitType)
    {
        return _unitTypeData.TryGetValue(unitType, out var value) && value.Attributes.Contains(Attribute.Structure);
    }
}

public interface IDataService
{
    public void OnStart(ResponseObservation obs, ResponseData? data, ResponseGameInfo? gameInfo);
    public bool IsStructure(uint unitType);
}