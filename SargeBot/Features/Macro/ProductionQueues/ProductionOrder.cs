using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.Macro.ProductionQueues
{
  public class ProductionOrder : IProductionOrder
  {
    public ProductionOrderType orderType { get; set; }
    public uint MineralCost { get; set; }
    public uint VespeneCost { get; set; }
    public float FoodRequired { get; set; }
    public IProductionSubQueue queue { get; set; }

    public ProductionOrder(ProductionOrderType orderType, uint MineralCost, 
      uint GasCost, float FoodRequired, IProductionSubQueue queue)
    {
      this.orderType = orderType;
      this.MineralCost = MineralCost;
      this.VespeneCost = GasCost;
      this.FoodRequired = FoodRequired;
      this.queue = queue;
    }


  }
}
