using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SargeBot;
using SargeBot.Features.GameData;
using SargeBot.Features.GameInfo;
using SargeBot.Features.Intel;
using SargeBot.Features.Macro;
using SargeBot.Features.Macro.Building.Zerg;
using SargeBot.Features.Micro;
using SargeBot.GameClients;
using SargeBot.Options;
using SC2APIProtocol;
using SC2ClientApi;

Console.WriteLine("Starting SargeBot");

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, config) => config.AddJsonFile("appsettings.json"))
    .ConfigureServices((context, services) =>
    {
        services.Configure<GameConnectionOptions>(context.Configuration.GetSection(GameConnectionOptions.GameConnection));
        services.Configure<RequestOptions>(context.Configuration.GetSection(RequestOptions.RequestSettings));
        services.Configure<CacheOptions>(context.Configuration.GetSection(CacheOptions.CacheSettings));
        services.Configure<ProcessOptions>(context.Configuration.GetSection(ProcessOptions.ProcessSettings));
        services
            .AddScoped<IGameEngine, GameEngine>()
            .AddScoped<MacroManager>()
            .AddScoped<ProcessOptions>()
            .AddScoped<StaticGameData>()
            .AddScoped<MapDataService>()
            .AddScoped<IntelService>()
            .AddScoped<ZergBuildingPlacement>()
            .AddScoped<MicroManager>()
            ;
    }).Build();

var gameSettings = host.Services.CreateGameSettings();

var playerOne = CreatePlayerClient(host.Services, gameSettings.PlayerOne, true);

var playerTwo = CreatePlayerClient(host.Services, gameSettings.PlayerTwo);

var game = new Game(playerOne, playerTwo);

// if args.length > 0 run ladder
await game.ExecuteMatch();

static GameClient? CreatePlayerClient(IServiceProvider services, PlayerSetup playerSetup, bool asHost = false)
{
    Console.WriteLine($"Creating {playerSetup.Type} {playerSetup.PlayerName}...");

    if (playerSetup.Type != PlayerType.Participant)
        return null;

    var serviceScope = services.CreateScope();
    var gameEngine = serviceScope.ServiceProvider.GetRequiredService<IGameEngine>();
    return new(services.CreateGameSettings(), gameEngine, playerSetup, asHost);
}