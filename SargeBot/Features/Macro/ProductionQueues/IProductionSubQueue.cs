using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = SC2APIProtocol.Action;

namespace SargeBot.Features.Macro.ProductionQueues
{
  public interface IProductionSubQueue
  {

    public Action? Activate(ResponseObservation observation);


  }
}
