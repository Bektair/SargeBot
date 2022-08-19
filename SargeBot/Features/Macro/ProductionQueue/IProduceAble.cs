using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.Macro.ProductionQueue
{
  public interface IProduceable
  {
    public uint MineralCost { get; set; }

    public uint VespeneCost { get; set; }

    public float FoodRequired { get; set; }

  }
}
