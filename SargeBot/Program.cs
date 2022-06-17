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

Log.Info("Starting SargeBot");

var isLadder = args.Length > 0;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, config) => config.AddJsonFile("appsettings.json"))
    .ConfigureServices((context, services) =>
    {
        services.Configure<CacheOptions>(context.Configuration.GetSection(nameof(CacheOptions)));
        services.Configure<GameOptions>(context.Configuration.GetSection(nameof(GameOptions)));
        services.Configure<Sc2ProcessOptions>(context.Configuration.GetSection(nameof(Sc2ProcessOptions)));
        services.Configure<ServerOptions>(context.Configuration.GetSection(nameof(ServerOptions)));
        if (isLadder)
            services.PostConfigure<ServerOptions>(options => ArgsToServerOptions(options, args));
        services
            .AddScoped<IGameEngine, SargeBotGameEngine>()
            .AddScoped<MacroManager>()
            .AddScoped<StaticGameData>()
            .AddScoped<MapDataService>()
            .AddScoped<IntelService>()
            .AddScoped<ZergBuildingPlacement>()
            .AddScoped<MicroManager>()
            ;
    }).Build();

var gameSettings = host.Services.CreateGameSettings();

var hostClient = CreateGameClient(host.Services, gameSettings.PlayerOne, true);

var guestClient = CreateGameClient(host.Services, gameSettings.PlayerTwo);

await Task.WhenAll(hostClient.Connect(), guestClient?.Connect() ?? Task.CompletedTask);

await hostClient.CreateGame();

await Task.WhenAll(hostClient.JoinGame(), guestClient?.JoinGame() ?? Task.CompletedTask);

await Task.WhenAll(hostClient.Run(), guestClient?.Run() ?? Task.CompletedTask);

static GameClient? CreateGameClient(IServiceProvider services, PlayerSetup playerSetup, bool isHost = false)
{
    Log.Info($"Creating {playerSetup.Type} {playerSetup.PlayerName}...");

    if (playerSetup.Type != PlayerType.Participant)
        return null;

    var serviceScope = services.CreateScope();
    var gameEngine = isHost ? serviceScope.ServiceProvider.GetRequiredService<IGameEngine>() : new ZeroBotGameEngine();
    return new(services.CreateGameSettings(), gameEngine, playerSetup, isHost);
}


void ArgsToServerOptions(ServerOptions options, string[] args)
{
    for (var i = 0; i < args.Length; i += 2)
        switch (args[i])
        {
            case "-g":
            case "--GamePort":
                options.GamePort = int.Parse(args[i + 1]);
                break;
            case "-o":
            case "--StartPort":
                options.StartPort = int.Parse(args[i + 1]);
                break;
            case "-l":
            case "--LadderServer":
                options.ServerAddress = args[i + 1];
                break;
            case "--OpponentId":
                options.OpponentId = args[i + 1];
                break;
        }
}