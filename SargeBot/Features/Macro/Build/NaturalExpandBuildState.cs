using SargeBot.Features.Macro.Building.Zerg;
using SargeBot.Features.Macro.ProductionQueues;
using SC2APIProtocol;
using SC2ClientApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.Macro.Build
{
  /// <summary>
  /// Makes drones and expansions
  /// </summary>
  public class NaturalExpandBuildState : BuildState
  {
    public NaturalExpandBuildState(BuildState state) : this(state.Observation, state.Build, state.ZergBuildingPlacement)
    {
    }
    public NaturalExpandBuildState(ResponseObservation observation, Build build, ZergBuildingPlacement zergBuildingPlacement)
    {
      this.observation = observation;
      this.build = build;
      this.ZergBuildingPlacement = zergBuildingPlacement;
    }

    public override SC2APIProtocol.Action ExecuteBuild(ProductionQueue queue, LarvaQueue larvaQueue)
    {
      if (!queue.ContainsUnit(UnitType.ZERG_DRONE) && larvaQueue.IsEmpty()) queue.EnqueueUnit(UnitType.ZERG_DRONE);

      if (!larvaQueue.IsEmpty()) {
        if (larvaQueue.CountLarvae(observation) > 0 && IProduceable.CanCreate(observation, larvaQueue.PeekLarva()))
        {
          queue.CreateUnitAction(observation, larvaQueue.Dequeue());
        }
      }
      else if (!queue.IsEmpty() && larvaQueue.IsEmpty()) { 
        if (IProduceable.CanCreate(observation, queue.Peek())){
          queue.ProduceFirstItem(observation);
        }
      }

      //Queue Needs to support buildings also, placement needs to find natural position.
      foreach (var unit in observation.Observation.RawData.Units)
      {
        if (unit.Alliance != Alliance.Self)
          continue;

        if (!unit.UnitType.Is(UnitType.ZERG_DRONE))
          continue;

        var command = new ActionRawUnitCommand();
        command.UnitTags.Add(unit.Tag);
        command.AbilityId = (int)Ability.BUILD_HATCHERY;
        command.TargetWorldSpacePos = _zergBuildingPlacement.FindPlacement();

        return new() { ActionRaw = new() { UnitCommand = command } };
      }

      return new() { ActionRaw = new() };
    }

    public override void NewObservations(ResponseObservation observation)
    {
      this.observation = observation;
      StateChangeCheck();
    }

    private void StateChangeCheck()
    { 
      
    
    }

    /// <summary>
    /// Changes state when you get 6 lings
    /// </summary>
    /// <returns></returns>
    public static Predicate<ResponseObservation> GetBuildState()
    {
      Predicate<ResponseObservation> predicate =
        (obs) => obs.Observation.RawData.Units.Count(u => u.UnitType.Is(UnitType.ZERG_ZERGLING)) >= 6;
      return predicate;
    }

  }
}
