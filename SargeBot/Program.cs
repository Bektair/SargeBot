using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SargeBot.Features.Debug;
using SargeBot.Features.Macro;
using SargeBot.GameClient;
using SargeBot.Options;
using SC2ClientApi;

Console.WriteLine("Starting SargeBot");

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, configuration) =>
    {
        var env = context.HostingEnvironment;
        configuration
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<GameConnectionOptions>(context.Configuration.GetSection(GameConnectionOptions.GameConnection));
        services.Configure<OpponentPlayerOptions>(context.Configuration.GetSection(OpponentPlayerOptions.OpponentPlayerSettings));
        services.Configure<RequestOptions>(context.Configuration.GetSection(RequestOptions.RequestSettings));
        services
            .AddSingleton(x => new GameClient(x.CreateGameSettings()))
            .AddSingleton<GameEngine>()
            .AddSingleton<DebugService>()
            .AddSingleton<MacroManager>()
            .AddSingleton<SystemSettings>()
            ;
    }).Build();

var gameEngine = host.Services.GetRequiredService<GameEngine>();

// if args.length > 0 run ladder
await gameEngine.RunSinglePlayer();

await host.RunAsync();