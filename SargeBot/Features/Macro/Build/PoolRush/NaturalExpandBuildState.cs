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
    public NaturalExpandBuildState(BuildState state) : base(state.Build, state.Observation)
    {
    }
    public NaturalExpandBuildState(Build build) : base(build)
    {
    }

    public override SC2APIProtocol.Action ExecuteBuild()
    {
      if (!_productionQueue.ContainsUnit(UnitType.ZERG_DRONE) && _larvaQueue.IsEmpty()) _productionQueue.EnqueueUnit(UnitType.ZERG_DRONE);

      if (!_larvaQueue.IsEmpty()) {
        if (_larvaQueue.CountLarvae(observation) > 0 && IProduceable.CanCreate(observation, _larvaQueue.PeekLarva()))
        {
          _productionQueue.CreateUnitAction(observation, _larvaQueue.Dequeue());
        }
      }
      else if (!_productionQueue.IsEmpty() && _larvaQueue.IsEmpty()) { 
        if (IProduceable.CanCreate(observation, _productionQueue.Peek())){
          _productionQueue.ProduceFirstItem(observation);
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
      currentState = PoolRush.AllBuildStates.NaturalExpand;
      base.StateChecker();
    }

    private void StateChangeCheck()
    { 
      
    
    }

    /// <summary>
    /// Changes state when you get 6 lings
    /// </summary>
    /// <returns></returns>
    public static Predicate<ResponseObservation> BuildPrecicate()
    {
      Predicate<ResponseObservation> predicate =
        (obs) => obs.Observation.RawData.Units.Count(u => u.UnitType.Is(UnitType.ZERG_ZERGLING)) >= 6;
      return predicate;
    }
  }
}
