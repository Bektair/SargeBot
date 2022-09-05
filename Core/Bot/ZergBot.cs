using SC2APIProtocol;

namespace Core.Bot;

public abstract class ZergBot : BaseBot
{
    protected ZergBot(IServiceProvider services) : base(services, Race.Zerg)
    {
    }
}