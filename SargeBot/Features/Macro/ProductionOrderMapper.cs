using SC2ClientApi.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.Macro;
/// <summary>
/// Go from unitType to ProductionType
/// </summary>
public class ProductionOrderMapper
{
    public static ProductionOrderType UnitTypesToProductionTypes (UnitTypes UnitTypes)
    {
        var prodType = ProductionOrderType.Unit;
        return prodType;
    }
    public static ProductionOrderType UpgradeToProductionTypes(UnitTypes UnitTypes)
    {
        var prodType = ProductionOrderType.Research;
        return prodType;
    }
}

