using Core.Data;
using Core.Intel;
using SC2APIProtocol;

namespace Core.Terran;

public class TerranIntelService : IntelService
{
    public TerranIntelService(IEnumerable<IDataService> dataServices) : base(dataServices)
    {
    }

    public override Race Race => Race.Terran;
}