using Microsoft.Extensions.DependencyInjection;
using SC2APIProtocol;

namespace Core.Zerg;

public abstract class ZergBot : BaseBot
{
    protected readonly ILarvaService LarvaService;
    protected readonly IOverlordService OverlordService;
    public readonly IZergBuildingPlacement ZergBuildingPlacement;


  protected ZergBot(IServiceProvider services) : base(services, Race.Zerg)
    {
        LarvaService = services.GetRequiredService<ILarvaService>();
        OverlordService = services.GetRequiredService<IOverlordService>();
        ZergBuildingPlacement = services.GetRequiredService<IZergBuildingPlacement>();
    }
}