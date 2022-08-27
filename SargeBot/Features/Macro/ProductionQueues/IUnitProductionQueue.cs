using SargeBot.Features.GameData;
using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = SC2APIProtocol.Action;

namespace SargeBot.Features.Macro.ProductionQueues;

public interface IUnitProductionQueue : IProductionSubQueue
{

  public void Enqueue(UnitType unit);

  public bool Contains(UnitType unit);

  public int CountOfUnitType(UnitType unit);

  public Action? CreateUnitAction(ResponseObservation obs, UnitType unitToMake);

  public bool ShouldMakeOverlord(ResponseObservation obs);

}
