using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SC2APIProtocol;
using SC2ClientApi;
using SC2ClientApi.Constants;
using ZeroBot;
using ZeroBot.Options;

Log.Info($"Starting ZeroBot");

var isLadder = args.Length > 0;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, config) => config.AddJsonFile("appsettings.json"))
    .ConfigureServices((context, services) =>
    {
        services.Configure<GameOptions>(context.Configuration.GetSection(nameof(GameOptions)));
        services.Configure<Sc2ProcessOptions>(context.Configuration.GetSection(nameof(Sc2ProcessOptions)));
        services.Configure<ServerOptions>(context.Configuration.GetSection(nameof(ServerOptions)));
        if (isLadder)
            services.PostConfigure<ServerOptions>(options => ArgsToServerOptions(options, args));
    }).Build();

var gs = host.Services.CreateGameSettings();

var playerOne = new GameClient(gs, new GameEngine(), gs.PlayerOne, isHost: !isLadder);

if (isLadder)
{
    try
    {
        await playerOne.ConnectToClient(20);
        await playerOne.JoinGame();
        await playerOne.Run();
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }
}
else
{
    if (!await playerOne.ConnectToClient(1))
    {
        Sc2Process.Start(gs, true);
        await Task.Delay(5000);
        await playerOne.ConnectToClient(20);
    }

    await playerOne.CreateGame();
    await playerOne.JoinGame();
    await playerOne.Run();
    
    //TODO: multiplayer
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