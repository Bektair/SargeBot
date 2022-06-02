using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.GameData;
public class Upgrade
{
    uint UpgradeId { get; set; } = 0;
    uint MineralCost { get; set; } = 0;
    uint VespeneCost { get; set; } = 0;
    uint AbilityId { get; set; } = 0;
    string Name { get; set; } = string.Empty;
    float ResearchTime { get; set; } = 0;
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
