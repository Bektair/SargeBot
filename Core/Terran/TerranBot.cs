using Microsoft.Extensions.DependencyInjection;
using SC2APIProtocol;

namespace Core.Terran;

public abstract class TerranBot : BaseBot
{
    protected TerranBot(IServiceProvider services) : base(services, Race.Terran)
    {
    }
}