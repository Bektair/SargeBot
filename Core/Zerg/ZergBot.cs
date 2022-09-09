using Microsoft.Extensions.DependencyInjection;
using SC2APIProtocol;

namespace Core.Zerg;

public abstract class ZergBot : BaseBot
{
    protected readonly IZergIntelService ZergIntelService;

    protected ZergBot(IServiceProvider services) : base(services, Race.Zerg)
    {
        ZergIntelService = services.GetRequiredService<IZergIntelService>();
    }
}