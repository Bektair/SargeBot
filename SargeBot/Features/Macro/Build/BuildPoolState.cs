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
      var canAffordSpawningPool = observation.Observation.PlayerCommon.Minerals >= 200;

      if (canAffordSpawningPool && !hasSpawningPool) { 
        foreach (var unit in observation.Observation.RawData.Units)
        {
          if (unit.Alliance != Alliance.Self)
            continue;

          if (!unit.UnitType.Is(UnitType.ZERG_DRONE))
            continue;

          var command = new ActionRawUnitCommand();
          command.UnitTags.Add(unit.Tag);
          command.AbilityId = (int)Ability.BUILD_SPAWNINGPOOL;
          command.TargetWorldSpacePos = _zergBuildingPlacement.FindPlacement();

          return new() { ActionRaw = new() { UnitCommand = command } };
        }
      }
      return new() { ActionRaw = new() };
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
