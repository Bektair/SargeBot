using SargeBot.Features.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.Macro.ProductionQueue
{
  public interface IUnitProductionQueue : IProductionSubQueue
  {

    public void Enqueue(UnitType unit);



  }
}
