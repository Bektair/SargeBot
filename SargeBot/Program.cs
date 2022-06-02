using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SargeBot;
using SargeBot.Features.Debug;
using SargeBot.Features.GameData;
using SargeBot.Features.GameInfo;
using SargeBot.Features.Macro;
using SargeBot.GameClients;
using SargeBot.Options;
using SC2APIProtocol;
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
        services.Configure<RequestOptions>(context.Configuration.GetSection(RequestOptions.RequestSettings));
        services.Configure<ProcessOptions>(context.Configuration.GetSection(ProcessOptions.ProcessSettings));
        services
            .AddScoped<IGameEngine, GameEngineTwo>()
            .AddScoped<DebugService>()
            .AddScoped<MacroManager>()
            .AddScoped<ProcessOptions>()
            .AddScoped<MapData>()
            .AddScoped<MapService>()
            .AddScoped<DataRequestManager>()
            .AddScoped<GameData>()
            ;
    }).Build();

var gameSettings = host.Services.CreateGameSettings();

var playerOne = CreatePlayerClient(host.Services, gameSettings.PlayerOne, true);

var playerTwo = gameSettings.IsMultiplayer()
    ? CreatePlayerClient(host.Services, gameSettings.PlayerTwo)
    : null;

var game = new Game(playerOne, playerTwo);

// if args.length > 0 run ladder
await game.ExecuteMatch();

await host.RunAsync();

static GameClient CreatePlayerClient(IServiceProvider services, PlayerSetup playerSetup, bool asHost = false)
{
    Console.WriteLine($"Creating player {playerSetup.PlayerName}...");

    var serviceScope = services.CreateScope();
    var gameEngine = serviceScope.ServiceProvider.GetRequiredService<IGameEngine>();
    return new(services.CreateGameSettings(), gameEngine, asHost);
}