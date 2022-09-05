using Core.Data;
using Core.Intel;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ServiceInstaller
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IDataService, DataService>();
        services.AddScoped<IIntelService, IntelService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IMicroService, MicroService>();
        services.AddScoped<IUnitService, UnitService>();
        return services;
    }
}