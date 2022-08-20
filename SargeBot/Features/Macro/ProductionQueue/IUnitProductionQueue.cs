using SargeBot.Features.GameData;
using SC2APIProtocol;
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

    public bool Contains(UnitType unit);

    public int CountOfUnitType(UnitType unit);

    public SC2APIProtocol.Action? CreateUnitAction(ResponseObservation obs, UnitType unitToMake);


  }
}
