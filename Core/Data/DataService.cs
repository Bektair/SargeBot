using SC2APIProtocol;
using Attribute = SC2APIProtocol.Attribute;

namespace Core.Data;

public abstract class DataService : IDataService
{
    private readonly Dictionary<uint, AbilityData> _abilityData = new();
    private readonly Dictionary<uint, BuffData> _buffData = new();
    private readonly Dictionary<uint, UnitTypeData> _unitTypeData = new();
    private readonly Dictionary<uint, UpgradeData> _upgradeData = new();

    public abstract Race Race { get; }

    public virtual void OnStart(ResponseObservation obs, ResponseData data, ResponseGameInfo gameInfo)
    {
        foreach (var ability in data.Abilities)
            _abilityData.Add(ability.AbilityId, ability);
        foreach (var buff in data.Buffs)
            _buffData.Add(buff.BuffId, buff);
        foreach (var upgrade in data.Upgrades)
            _upgradeData.Add(upgrade.UpgradeId, upgrade);
        foreach (var unitType in data.Units)
            _unitTypeData.Add(unitType.UnitId, unitType);
    }

    public virtual bool IsStructure(uint unitType)
    {
        return _unitTypeData.TryGetValue(unitType, out var value) && value.Attributes.Contains(Attribute.Structure);
    }
}

public interface IDataService
{
    public Race Race { get; }
    public void OnStart(ResponseObservation obs, ResponseData data, ResponseGameInfo gameInfo);
    public bool IsStructure(uint unitType);
}