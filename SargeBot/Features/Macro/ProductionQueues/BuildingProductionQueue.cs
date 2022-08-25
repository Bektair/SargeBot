using SargeBot.Features.GameData;
using SargeBot.Features.Macro.Building.Zerg;
using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = SC2APIProtocol.Action;

namespace SargeBot.Features.Macro.ProductionQueues
{
  public class BuildingProductionQueue : IBuildingProductionQueue
  {
    Queue<Ability> BuildingQueue;
    StaticGameData _staticGameData;
    ZergBuildingPlacement _zergBuildingPlacement;

    public BuildingProductionQueue(StaticGameData staticGameData, ZergBuildingPlacement zergBuildingPlacement)
    {
      _staticGameData = staticGameData;
      _zergBuildingPlacement = zergBuildingPlacement;
    }
    public Action? Activate(ResponseObservation observation)
    {




      return null;

    }

    public void Enqueue(Ability ability)
    {
      BuildingQueue.Enqueue(ability);
    }
  }
}
