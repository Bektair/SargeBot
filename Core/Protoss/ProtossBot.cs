using Core.Bot;
using SC2APIProtocol;

namespace Core.Protoss;

public abstract class ProtossBot : BaseBot
{
    protected ProtossBot(IServiceProvider services) : base(services, Race.Protoss)
    {
    }
}