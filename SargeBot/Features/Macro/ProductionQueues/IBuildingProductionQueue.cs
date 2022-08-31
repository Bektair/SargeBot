using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.Macro.ProductionQueues
{
  public interface IBuildingProductionQueue : IProductionSubQueue
  {
    public HashSet<ulong> getBuilders();

    public Unit? builderDroneOfAbillity(Ability ability);

    public void Enqueue(Ability ability);

    public bool Contains(Ability ability);


  }
}
