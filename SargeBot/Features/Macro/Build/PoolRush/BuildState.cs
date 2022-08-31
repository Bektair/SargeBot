using SargeBot.Features.Macro.Build.PoolRush;
using SargeBot.Features.Macro.Building.Zerg;
using SargeBot.Features.Macro.ProductionQueues;
using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = SC2APIProtocol.Action;

namespace SargeBot.Features.Macro.Build;

/// <summary>
/// State Context
/// </summary>
public abstract class BuildState
{

  /// <summary>
  /// I need to be able to say these states are todays selection, without using them as instances
  /// </summary>

  protected AllBuildStates currentState = AllBuildStates.BuildPool;

  protected Build build;
  protected ResponseObservation observation;

  protected ZergBuildingPlacement _zergBuildingPlacement { get; set; }
  protected ProductionQueue _productionQueue { get; set; }
  protected LarvaQueue _larvaQueue { get; set; }

  public BuildState(ZergBuildingPlacement zergBuildingPlacement, ProductionQueue productionQueue, LarvaQueue larvaQueue)
  {
    _zergBuildingPlacement = zergBuildingPlacement;
    _productionQueue = productionQueue;
    _larvaQueue = larvaQueue;
    
  }

  public BuildState(Build build) : this(build._zergBuildingPlacement,build._productionQueue, build._larvaQueue)
  {
    this.build = build;
  }

  public BuildState(Build build, ResponseObservation observation) : this(build._zergBuildingPlacement, build._productionQueue, build._larvaQueue)
  {
    this.observation = observation;
    this.build = build;
  }
  public BuildState(BuildState state) : this(state.Build, state.Observation)
  {

  }

  protected void StateChecker()
  {
    //foreach of the selected enums 
    //Maby select them with a predicate attached
    //If the predicate happens then it transitions.
    foreach(var enumState in build._statesAndPredicates)
    {
      if (currentState == enumState.Key) continue;
      bool enumStateValue = enumState.Value.Invoke(observation);
      if (enumStateValue)
      {
        build.State = BuildStateFactory.CreateState(enumState.Key, this);
      }
    }
  }

  public ResponseObservation Observation
  {
    get { return observation; }
    set { observation = value; }
  }

  public Build Build
  {
    get { return build; }
    set { build = value; }
  }

 

  /// <summary>
  /// Changes the state
  /// </summary>
  /// <param name="observation"></param>
  public abstract void NewObservations(ResponseObservation observation);

  /// <summary>
  /// Acts based on the state
  /// </summary>
  /// <param name="observation"></param>
  public abstract Action ExecuteBuild();

}
