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
        services.AddScoped<IIntelService, IntelService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IMicroService, MicroService>();
        services.AddScoped<IUnitService, UnitService>();
        return services;
    }
}