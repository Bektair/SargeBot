using SargeBot.Features.Macro.Build.PoolRush;
using SargeBot.Features.Macro.Building.Zerg;
using SargeBot.Features.Macro.ProductionQueues;
using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.Macro.Build;

public class Build
{

  //Clearly I want it to change behaviour based on the amount of lings and more that is based on observation
  //Each state holds a check that tells if they should change to the other stages
  public BuildState State { get; set; }

  public ZergBuildingPlacement _zergBuildingPlacement { get; set; }
  public ProductionQueue _productionQueue { get; set; }
  public LarvaQueue  _larvaQueue{ get; set; }
  public Dictionary<AllBuildStates, Predicate<ResponseObservation>> _statesAndPredicates = new();
  protected ResponseObservation observation { get { return State.Observation; }  }
  public Build(ZergBuildingPlacement zergBuildingPlacement, ProductionQueue productionQueue, LarvaQueue larvaQueue)
  {
    _zergBuildingPlacement = zergBuildingPlacement;
    _productionQueue = productionQueue;
    _larvaQueue = larvaQueue;

    _statesAndPredicates.Add(AllBuildStates.BuildPool, BuildPoolState.BuildPrecicate());
    _statesAndPredicates.Add(AllBuildStates.BuildLing, BuildLingState.BuildPrecicate());
    _statesAndPredicates.Add(AllBuildStates.NaturalExpand, NaturalExpandBuildState.BuildPrecicate());
    
    this.State = new BuildPoolState(this);
  }
}
