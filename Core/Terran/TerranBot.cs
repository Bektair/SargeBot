using Core.Bot;
using Microsoft.Extensions.DependencyInjection;
using SC2APIProtocol;

namespace Core.Terran;

public abstract class TerranBot : BaseBot
{
    protected readonly IUnitService UnitService;

    protected TerranBot(IServiceProvider services) : base(services, Race.Terran)
    {
        // get terran unit service
        UnitService = services.GetRequiredService<IUnitService>();
    }
}