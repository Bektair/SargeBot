// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Net.WebSockets;
using SargeBot;
using SC2APIProtocol;
using SargeBot.GameClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");




string address = "127.0.0.1";
int port = 5678;

using IHost host = CreateHostBuilder(args, address, port).Build();

static IHostBuilder CreateHostBuilder(string[] args, string address, int port) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) => 
                services.AddTransient<IGameConnection>(sp => new GameConnection(address, port))
                .AddTransient<SC2Process>());
            
 _ = host.Services.GetService<SC2Process>();

var process = ActivatorUtilities.CreateInstance<SC2Process>(host.Services);
IGameConnection connection = process.GetGameConnection();
await connection.Connect();


var map = "HardwireAIE.SC2Map";
var opponentRace = Race.Protoss;
var opponentDifficulty = Difficulty.CheatInsane;
var aIBuild = AIBuild.Rush;
var randomSeed = -1;
await connection.CreateGame(process.getMapPath(map), opponentRace, opponentDifficulty, aIBuild, randomSeed);
var playerId = await connection.JoinGame(opponentRace);
await connection.Run(null, playerId, "test");


await host.RunAsync();


Console.WriteLine("Hello, World!");
