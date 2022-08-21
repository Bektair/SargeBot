using SargeBot.Features.Macro.Building.Zerg;
using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.Macro.Build
{
  public class Build
  {

    //Clearly I want it to change behaviour based on the amount of lings and more that is based on observation
    //Each state holds a check that tells if they should change to the other stages
    public BuildState State { get; set; }
    ZergBuildingPlacement _zergBuildingPlacement;
    protected ResponseObservation observation { get { return State.Observation; }  }
    public Build(ZergBuildingPlacement zergBuildingPlacement)
    {
      _zergBuildingPlacement = zergBuildingPlacement;
      this.State = new BuildPoolState(this, _zergBuildingPlacement);
    }
  }
}
