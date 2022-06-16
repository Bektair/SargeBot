using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SC2APIProtocol;
using SC2ClientApi;
using SC2ClientApi.Constants;
using ZeroBot;
using ZeroBot.GameClients;
using ZeroBot.Options;

Console.WriteLine("Starting ZeroBot");

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

var playerOne = new GameClient(gs, new GameEngine(), !isLadder);

if (isLadder)
{
    try
    {
        await playerOne.ConnectToClient(20);
        var joinResponse = await playerOne.JoinLadderGameRequest(Race.Zerg, gs.ConnectionServerPort);
        await playerOne.Run(joinResponse.JoinGame.PlayerId);
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
        var folderPath = @"C:\Program Files (x86)\StarCraft II";
        var sc2Exe = Directory.GetDirectories(folderPath + @"\Versions\", @"Base*")[0] + @"\SC2_x64.exe";
        var workingDirectory = $"{folderPath}\\Support64";
        var arguments =
            $"{ClientConstants.Address} {gs.ConnectionAddress} {ClientConstants.Port} {gs.ConnectionServerPort} {ClientConstants.Fullscreen} 0 {ClientConstants.WindowWidth} 800 {ClientConstants.WindowHeight} 600 {ClientConstants.WindowX} 20 {ClientConstants.WindowY} 30 ";
        playerOne.LaunchClient(sc2Exe, arguments, workingDirectory);
        await Task.Delay(5000);
        await playerOne.ConnectToClient(20);
    }

    await playerOne.CreateGameRequest();
    await playerOne.JoinLadderGameRequest(gs.PlayerOne.Race, 0);
    await playerOne.Run(1);
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
                options.LadderServer = args[i + 1];
                break;
            case "--OpponentId":
                options.OpponentId = args[i + 1];
                break;
        }
}