using System.Text.Json.Serialization;
using SargeBot.Features.Macro.ProductionQueues;
using SC2APIProtocol;

namespace SargeBot.Features.GameData;

public class PlainUpgrade : IProduceable
{
    [JsonConstructor]
    public PlainUpgrade(string Name, uint UpgradeId, uint MineralCost, uint VespeneCost, uint AbilityId, float ResearchTime)
    {
        this.Name = Name;
        this.UpgradeId = UpgradeId;
        this.MineralCost = MineralCost;
        this.VespeneCost = VespeneCost;
        this.AbilityId = AbilityId;
        this.ResearchTime = ResearchTime;
    }

    public PlainUpgrade(UpgradeData upgradeData)
    {
        UpgradeId = upgradeData.UpgradeId;
        MineralCost = upgradeData.MineralCost;
        VespeneCost = upgradeData.VespeneCost;
        AbilityId = upgradeData.AbilityId;
        Name = upgradeData.Name;
        ResearchTime = upgradeData.ResearchTime;
    }

    public string Name { get; set; } = string.Empty;
    public uint UpgradeId { get; set; }
    public uint MineralCost { get; set; }
    public uint VespeneCost { get; set; }
    public uint AbilityId { get; set; }
    public float ResearchTime { get; set; }
    public float FoodRequired { get; set; } = 0;
}