using SargeBot.Features.Macro.Building.Zerg;
using SargeBot.Features.Macro.ProductionQueues;
using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = SC2APIProtocol.Action;

namespace SargeBot.Features.Macro.Build
{
  public abstract class BuildState
  {
    //When you have x amount of lings you go to the next state
    protected Build build;
    protected ResponseObservation observation;
    protected ZergBuildingPlacement _zergBuildingPlacement;

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

    public ZergBuildingPlacement ZergBuildingPlacement
    {
      get { return _zergBuildingPlacement; }
      set { _zergBuildingPlacement = value; }
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
    public abstract Action ExecuteBuild(ProductionQueue queue, LarvaQueue larvaQueue);
  }
}
