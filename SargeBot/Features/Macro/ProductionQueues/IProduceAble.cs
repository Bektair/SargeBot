using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.Macro.ProductionQueues
{
  public interface IProduceable
  {
    public uint MineralCost { get; }

    public uint VespeneCost { get; }

    public float FoodRequired { get; }

    public static bool CanCreate(ResponseObservation obs, IProduceable type)
    {
      uint minerals = obs.Observation.PlayerCommon.Minerals;
      uint Vespene = obs.Observation.PlayerCommon.Vespene;
      uint Supply = obs.Observation.PlayerCommon.FoodCap;
      uint SupplyUsed = obs.Observation.PlayerCommon.FoodUsed;

      if (type.MineralCost > minerals) return false;
      if (type.VespeneCost > Vespene) return false;
      if (type.FoodRequired > Supply-SupplyUsed) return false;

      return true;
    }
  }
}
