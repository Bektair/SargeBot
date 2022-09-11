using Core.Protoss;
using Core.Terran;
using Core.Zerg;
using Microsoft.Extensions.DependencyInjection;
using Core.Intel;

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
        services.AddScoped<MapDataService>();

        // find better solution than this >_>
        services.AddScoped(x => x.GetRequiredService<IEnumerable<IIntelService>>().FirstOrDefault(y => y is IZergIntelService) as IZergIntelService);
        return services;
    }
}