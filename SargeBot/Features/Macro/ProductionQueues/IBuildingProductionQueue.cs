﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.Macro.ProductionQueues
{
  public interface IBuildingProductionQueue : IProductionSubQueue
  {

    public void Enqueue(Ability ability);



  }
}
