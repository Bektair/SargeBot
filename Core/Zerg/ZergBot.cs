using Microsoft.Extensions.DependencyInjection;
using SC2APIProtocol;

namespace Core.Zerg;

public abstract class ZergBot : BaseBot
{

    public readonly IZergBuildingPlacement ZergBuildingPlacement;


  protected ZergBot(IServiceProvider services) : base(services, Race.Zerg)
    {

        ZergBuildingPlacement = services.GetRequiredService<IZergBuildingPlacement>();
    }
}