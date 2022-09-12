using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions;

public static class AbilityExtensions
{
  //
  

  public static bool IsWorkerBuildAbillity(this uint id)
  {
    return id is
      1162 or 1156 or 1154 or 1152 or 1157 or 1160 or 1161 or 1165 or 1155 or 1166 or 1158 or 1167 or 1159 // Zerg build
      or 331 or 321 or 324 or 318 or 322 or 328 or 333 or 327 or 320 or 323 or 326 or 319 or 329 // Terran build
      or 882 or 894 or 891 or 885 or 884 or 883 or 880 or 887 or 881 or 892 or 893 or 889 or 890 or 886 or 895; //Protoss build
  }




}
