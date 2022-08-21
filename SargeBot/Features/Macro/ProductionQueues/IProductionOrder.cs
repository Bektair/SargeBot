using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.Macro.ProductionQueues
{
  public interface IProductionOrder : IProduceable
  {

    public ProductionOrderType orderType { get; set; }

    public IProductionSubQueue queue { get; set; }


  }
}
