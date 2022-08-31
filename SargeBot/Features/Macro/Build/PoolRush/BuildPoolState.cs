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
    public BuildPoolState(BuildState state) : base(state)
    {
    }

    public BuildPoolState(Build build) : base(build)
    {
    }

    public override void NewObservations(ResponseObservation observation)
    {
      this.observation = observation;
      base.currentState = PoolRush.AllBuildStates.BuildPool;
      base.StateChecker();   
    }

    public override Action ExecuteBuild()
    {
      var hasSpawningPool = observation.Observation.RawData.Units.Any(u => u.UnitType.Is(UnitType.ZERG_SPAWNINGPOOL));
      //I still need to find out a way to know if there is a drone with an order >_> this must be a candidate for observer pattern

      if (!hasSpawningPool && !_productionQueue.ContainsBuilding(Ability.BUILD_SPAWNINGPOOL))
      {
        _productionQueue.Clear();
        _productionQueue.EnqueueBuilding(UnitType.ZERG_SPAWNINGPOOL);
      }
      if (!_productionQueue.IsEmpty() && !hasSpawningPool)
      {
        var canAffordSpawningPool = IProduceable.CanCreate(observation, _productionQueue.Peek());
        if(canAffordSpawningPool)
          return _productionQueue.ProduceFirstItem(observation);
      }

      if (hasSpawningPool) { 
        Action? shouldMakeOv = _productionQueue.TryProduceOv(observation);
        if (shouldMakeOv != null)
        {
          return shouldMakeOv;
        }
      }

      return new Action() { };
    }


    /// <summary>
    /// Goes into this state if the spawning pool is missing
    /// </summary>
    /// <returns></returns>
    public static Predicate<ResponseObservation> BuildPrecicate()
    {
      Predicate<ResponseObservation> predicate =
        (obs) => !obs.Observation.RawData.Units.Any(u => u.UnitType.Is(UnitType.ZERG_SPAWNINGPOOL));
      return predicate;
    }
  }
}
