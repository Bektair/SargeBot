using Core.Data;
using Core.Intel;
using SC2APIProtocol;

namespace Core.Protoss;

public class ProtossIntelService : IntelService
{
    public ProtossIntelService(IEnumerable<IDataService> dataServices) : base(dataServices)
    {
    }

    public override Race Race => Race.Protoss;
}