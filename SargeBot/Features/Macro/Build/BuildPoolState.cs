using SargeBot.Features.Macro.Building.Zerg;
using SargeBot.Features.Macro.ProductionQueues;
using SC2APIProtocol;
using SC2ClientApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = SC2APIProtocol.Action;

namespace SargeBot.Features.Macro.Build
{
  /// <summary>
  /// This is the initial state, and thus has multiple constructors
  /// </summary>
  public class BuildPoolState : BuildState
  {
    public BuildPoolState(ResponseObservation obs, Build build, ZergBuildingPlacement zergBuildingPlacement)
    {
      this.observation = obs;
      this.build = build;
      this.ZergBuildingPlacement = zergBuildingPlacement;
    }

    public BuildPoolState(Build build, ZergBuildingPlacement zergBuildingPlacement)
    {
      this.build = build;
      this.ZergBuildingPlacement = zergBuildingPlacement;
    }

    public BuildPoolState(BuildState buildState) : this(buildState.Observation, buildState.Build, buildState.ZergBuildingPlacement) 
    {

    }
    
    public override void NewObservations(ResponseObservation observation)
    {
      this.observation = observation;
      StateChangeCheck();
    }

    public override Action ExecuteBuild(ProductionQueue _productionQueue, LarvaQueue larvaQueue)
    {
      var hasSpawningPool = observation.Observation.RawData.Units.Any(u => u.UnitType.Is(UnitType.ZERG_SPAWNINGPOOL));

      if (!hasSpawningPool && !_productionQueue.ContainsUnit(UnitType.ZERG_SPAWNINGPOOL))
      {
        _productionQueue.Clear();
        _productionQueue.EnqueueBuilding(UnitType.ZERG_SPAWNINGPOOL);
      }
      if (!_productionQueue.IsEmpty())
      {
        var canAffordSpawningPool = IProduceable.CanCreate(observation, _productionQueue.Peek());
        if(canAffordSpawningPool)
          return _productionQueue.ProduceFirstItem(observation);
      }

      return new Action() { };
    }

    private void StateChangeCheck()
    {
      Predicate<ResponseObservation> lingcheck = BuildLingState.GetBuildState();
      if (lingcheck.Invoke(observation))
      {
        build.State = new BuildLingState(this);
      }
    }

    public static Predicate<ResponseObservation> GetBuildState()
    {
      Predicate<ResponseObservation> predicate =
        (obs) => !obs.Observation.RawData.Units.Any(u => u.UnitType.Is(UnitType.ZERG_SPAWNINGPOOL));
      return predicate;
    }

  }
}
