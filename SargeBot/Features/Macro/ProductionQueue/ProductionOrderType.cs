using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.Macro.ProductionQueue { 
/// <summary>
/// A combined enum for all types that can be produced
/// </summary>
public enum ProductionOrderType
{
    Research = 0,
    Unit = 1,
    Addon = 2,
    Morphed = 3,
    Structure = 4
}

}