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
  public class BuildLingState : BuildState
  {
    public BuildLingState(BuildState state) : base(state)
    {
    }

    public BuildLingState(Build build) : base(build)
    {
    }

    public override Action ExecuteBuild()
    {
      var hasSpawningPoolComplete = observation.Observation.RawData.Units.Any(u => u.UnitType.Is(UnitType.ZERG_SPAWNINGPOOL) && u.BuildProgress > 0.9999999);
      if (hasSpawningPoolComplete)
      {
        var lingCount = observation.Observation.RawData.Units.Count(u => u.UnitType.Is(UnitType.ZERG_ZERGLING));
        int lingsInQueues = _productionQueue.CountInstancesOfUnit(UnitType.ZERG_ZERGLING);
        int lingsInEggs = _larvaQueue.EggsOfAbillityId(observation, Ability.TRAIN_ZERGLING);
        if (lingCount + lingsInQueues * 2 + lingsInEggs * 2 < 6)
        {
          _productionQueue.EnqueueUnit(UnitType.ZERG_ZERGLING);
        }
        else
        {

        }
      }

      uint minerals = observation.Observation.PlayerCommon.Minerals;
      uint gas = observation.Observation.PlayerCommon.Vespene;
      bool larvaCreate = false;
      if (!_larvaQueue.IsEmpty())
      {
        if (_larvaQueue.CanCreate(observation))
        {
          return _productionQueue.CreateUnitAction(observation, _larvaQueue.Dequeue());
        }
      }

      if (!_productionQueue.IsEmpty() && !larvaCreate)
      {
        if (minerals > _productionQueue.Peek().MineralCost)
        {
          return _productionQueue.ProduceFirstItem(observation);
        }
      }
      return new Action() { };

    }

    public override void NewObservations(ResponseObservation observation)
    {
      this.observation = observation;
      base.currentState = PoolRush.AllBuildStates.BuildLing;
      base.StateChecker();
    }

    /// <summary>
    /// Each state has a rule a kind of function or delegate that tells you which state to choose
    /// 
    /// </summary>
    private void StateChangeCheck()
    {
      //Lingcheck
/*     Predicate<ResponseObservation> lingcheck = BuildLingState.GetBuildState();
      Predicate<ResponseObservation> poolCheck = BuildPoolState.GetBuildState();
      Predicate<ResponseObservation> naturalcheck = NaturalExpandBuildState.GetBuildState();

      if (poolCheck.Invoke(observation)) {
        build.State = new BuildPoolState(this);
      }
      else if (naturalcheck.Invoke(observation))
      {
        build.State = new NaturalExpandBuildState(this);
      }*/

    }

    /// <summary>
    /// Goes into this state when the pool is done
    /// </summary>
    /// <returns></returns>
    public static Predicate<ResponseObservation> BuildPrecicate()
    {
      Predicate<ResponseObservation> predicate =
        (obs) => obs.Observation.RawData.Units.Any(u => u.UnitType.Is(UnitType.ZERG_SPAWNINGPOOL) && u.BuildProgress == 1);
      return predicate;
    }

  }
}
