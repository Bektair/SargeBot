namespace SargeBot.Features.Macro.ProductionQueues;

/// <summary>
///     Go from unitType to ProductionType
/// </summary>
public class ProductionOrderMapper
{
    public static ProductionOrderType UnitTypesToProductionTypes(UnitType UnitTypes)
    {
        var prodType = ProductionOrderType.Unit;
        return prodType;
    }

    public static ProductionOrderType UpgradeToProductionTypes(UnitType UnitTypes)
    {
        var prodType = ProductionOrderType.Research;
        return prodType;
    }
}