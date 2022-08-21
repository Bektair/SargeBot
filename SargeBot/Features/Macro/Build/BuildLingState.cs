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
    public BuildLingState(ResponseObservation observation, Build build, ZergBuildingPlacement zergBuildingPlacement)
    {
      this.observation = observation;
      this.build = build;
      this.ZergBuildingPlacement = zergBuildingPlacement;
    }

    public BuildLingState(BuildState state) : this(state.Observation, state.Build, state.ZergBuildingPlacement)
    {
    }

    public override Action ExecuteBuild(ProductionQueue _productionQueue, LarvaQueue _larvaQueue)
    {
      var hasSpawningPoolComplete = observation.Observation.RawData.Units.Any(u => u.UnitType.Is(UnitType.ZERG_SPAWNINGPOOL) && u.BuildProgress > 0.9999999);
      if (hasSpawningPoolComplete)
      {
        var lingCount = observation.Observation.RawData.Units.Count(u => u.UnitType.Is(UnitType.ZERG_ZERGLING));
        int lingsInQueues = _productionQueue.CountInstancesOfUnit(UnitType.ZERG_ZERGLING);
        int lingsInEggs = LarvaQueue.EggsOfAbillityId(observation, Ability.TRAIN_ZERGLING);
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
      StateChangeCheck();
    }

    /// <summary>
    /// Each state has a rule a kind of function or delegate that tells you which state to choose
    /// 
    /// </summary>
    private void StateChangeCheck()
    {
      //Lingcheck
      Predicate<ResponseObservation> lingcheck = BuildLingState.GetBuildState();
      if (lingcheck.Invoke(observation))
      {
        //No changes
      }

    }
    public static Predicate<ResponseObservation> GetBuildState()
    {
      Predicate<ResponseObservation> predicate = 
        (obs)=>obs.Observation.RawData.Units.Any(u => u.UnitType.Is(UnitType.ZERG_SPAWNINGPOOL) && u.BuildProgress > 0.9999999);
      return predicate;
    }



  }
}
