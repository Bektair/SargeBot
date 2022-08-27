using SargeBot.Features.GameData;
using SargeBot.Features.Macro.Building.Zerg;
using SC2APIProtocol;
using SC2ClientApi;
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
    Queue<Ability> _BuildingQueue;
    StaticGameData _staticGameData;
    ZergBuildingPlacement _zergBuildingPlacement;

    public BuildingProductionQueue(StaticGameData staticGameData, ZergBuildingPlacement zergBuildingPlacement)
    {
      _staticGameData = staticGameData;
      _zergBuildingPlacement = zergBuildingPlacement;
      _BuildingQueue = new Queue<Ability>();
    }
    public Action? Activate(ResponseObservation observation)
    {
      Ability createBuildingAbillity = _BuildingQueue.Peek();
      foreach (var unit in observation.Observation.RawData.Units)
      {
        if (unit.Alliance != Alliance.Self)
          continue;

        if (!unit.UnitType.Is(UnitType.ZERG_DRONE))
          continue;

        var command = new ActionRawUnitCommand();
        command.UnitTags.Add(unit.Tag);
        command.AbilityId = (int)createBuildingAbillity;
        command.TargetWorldSpacePos = _zergBuildingPlacement.FindPlacement();
        _BuildingQueue.Dequeue();

        return new() { ActionRaw = new() { UnitCommand = command } };
      }

      return new() { ActionRaw = new() };
    }

    public void Enqueue(Ability ability)
    {
      _BuildingQueue.Enqueue(ability);
    }
  }
}
