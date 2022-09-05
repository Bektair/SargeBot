using Core.Bot;
using SC2APIProtocol;

namespace Core.Zerg;

public abstract class ZergBot : BaseBot
{
    protected ZergBot(IServiceProvider services) : base(services, Race.Zerg)
    {
    }
}