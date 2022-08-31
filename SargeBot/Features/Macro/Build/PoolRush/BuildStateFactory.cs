using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SargeBot.Features.Macro.Build.BuildState;

namespace SargeBot.Features.Macro.Build.PoolRush;
public enum AllBuildStates { BuildPool, BuildLing, NaturalExpand, Empty }; //Somehow translated into which state to use

/// <summary>
/// Could be made abstract to differentiate between builds for other factions.
/// </summary>
public class BuildStateFactory
{

  public static BuildState CreateState(AllBuildStates states, BuildState previousState)
  {
    switch (states)
    {
      case AllBuildStates.BuildPool:
        {
          return new BuildPoolState(previousState);
        }
      case AllBuildStates.BuildLing:
        {
          return new BuildLingState(previousState);
        }
      case AllBuildStates.NaturalExpand:
        {
          return new NaturalExpandBuildState(previousState);
        }
      default: return new BuildPoolState(previousState);
    }

  }


}
