using Core.Data;
using Core.Intel;
using Core.Protoss;
using Core.Terran;
using Core.Zerg;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ServiceInstaller
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IDataService, ZergDataService>();
        services.AddScoped<IDataService, ProtossDataService>();
        services.AddScoped<IDataService, TerranDataService>();
        services.AddScoped<IIntelService, ZergIntelService>();
        services.AddScoped<IIntelService, ProtossIntelService>();
        services.AddScoped<IIntelService, TerranIntelService>();
        services.AddScoped<IMacroService, ZergMacroService>();
        services.AddScoped<IMacroService, ProtossMacroService>();
        services.AddScoped<IMacroService, TerranMacroService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IMicroService, MicroService>();
        services.AddScoped<IZergBuildingPlacement, ZergBuildingPlacement>();

        // find better solution than this >_>
        services.AddScoped<ILarvaService>(x => x.GetRequiredService<IEnumerable<IIntelService>>().FirstOrDefault(y => y is ZergIntelService) as ZergIntelService);
        services.AddScoped<IOverlordService>(x => x.GetRequiredService<IEnumerable<IIntelService>>().FirstOrDefault(y => y is ZergIntelService) as ZergIntelService);
        return services;
    }
}