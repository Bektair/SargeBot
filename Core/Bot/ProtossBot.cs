using SC2APIProtocol;

namespace Core.Bot;

public abstract class ProtossBot : BaseBot
{
    protected ProtossBot(IServiceProvider services) : base(services, Race.Protoss)
    {
    }
}