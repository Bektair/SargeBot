using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SargeBot.Features.GameData;
public class Upgrade
{
    public string Name { get; set; } = string.Empty;
    public uint UpgradeId { get; set; } = 0;
    public uint MineralCost { get; set; } = 0;
    public uint VespeneCost { get; set; } = 0;
    public uint AbilityId { get; set; } = 0;
    public float ResearchTime { get; set; } = 0;

    [JsonConstructor]
    public Upgrade(string Name, uint UpgradeId, uint MineralCost, uint VespeneCost, uint AbilityId, float ResearchTime)
    {
        this.Name = Name;
        this.UpgradeId = UpgradeId;
        this.MineralCost = MineralCost;
        this.VespeneCost = VespeneCost;
        this.AbilityId = AbilityId;
        this.ResearchTime = ResearchTime;
    }
    public Upgrade(UpgradeData upgradeData)
    {
        UpgradeId = upgradeData.UpgradeId;
        MineralCost = upgradeData.MineralCost;
        VespeneCost = upgradeData.VespeneCost;
        AbilityId = upgradeData.AbilityId;
        Name = upgradeData.Name;
        ResearchTime = upgradeData.ResearchTime;
    }





}
