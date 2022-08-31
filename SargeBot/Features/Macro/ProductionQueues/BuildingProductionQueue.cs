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

    HashSet<ulong> builderDrones = new() { };
    List<Unit> builderDronesUnit = new() { };

    public static HashSet<uint> zergBuildAbillities = new()
    {
      1162, 1156, 1154, 1152, 1157, 1160, 1161, 1165, 1155, 1166, 1158, 1167, 1159 //Zerg build abillities BUILD_*
    };
    


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

        //TODO: Use observer pattern to maintain a list of builderDrones, 
        //This list deletes drones if they are dead or has no order
        builderDrones.Add(unit.Tag);
        var command = new ActionRawUnitCommand();
        command.UnitTags.Add(unit.Tag);
        command.AbilityId = (int)createBuildingAbillity;
        command.TargetWorldSpacePos = _zergBuildingPlacement.FindPlacement();
        _BuildingQueue.Dequeue();
        CleanBuilderList(observation);

        return new() { ActionRaw = new() { UnitCommand = command } };
      }

      return new() { ActionRaw = new() };
    }

    /// <summary>
    /// I think this works to check if the drone is building.
    /// </summary>
    /// <param name="observation"></param>
    private void CleanBuilderList(ResponseObservation observation)
    {
      if (builderDrones.Count == 0) return;
        builderDronesUnit = observation.Observation.RawData.Units.Where(unit => builderDrones.Contains(unit.Tag)).ToList();
      builderDrones = builderDronesUnit
        .Where(builder => builder.Health > 0 && builder.Orders.Where(order=> zergBuildAbillities.Contains(order.AbilityId)).Any())
        .Select(builder => builder.Tag)
        .ToHashSet();
    }

    public HashSet<ulong> getBuilders()
    {
      return builderDrones;
    }

    public Unit? builderDroneOfAbillity(Ability ability)
    {
      return builderDronesUnit
        .Where(builder=> builder.Orders.Where(order => (ulong)ability == order.AbilityId).Any())
        .FirstOrDefault();
    }

    public bool Contains(Ability ability)
    {
      return _BuildingQueue.Contains(ability) && builderDroneOfAbillity != null;
    }

    public void Enqueue(Ability ability)
    {
      _BuildingQueue.Enqueue(ability);
    }

  }
}
