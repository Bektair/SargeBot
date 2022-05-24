// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Net.WebSockets;
using SargeBot;
using SC2APIProtocol;
using SargeBot.GameClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SargeBot.Options;
using Microsoft.Extensions.Options;

Console.WriteLine("Hello, World!");


using IHost host = CreateHostBuilder(args)
    .Build();

static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingcontext, configuration) =>
            {
                configuration.Sources.Clear();
                IHostEnvironment env = hostingcontext.HostingEnvironment;
                configuration
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

                IConfigurationRoot configurationRoot = configuration.Build();
                GameConnectionOptions options = new();
                configurationRoot.GetSection(GameConnectionOptions.GameConnection)
                                 .Bind(options);
            })
            .ConfigureServices((context, services) =>
            {

                var configurationRoot = context.Configuration;
                GameConnectionOptions options = new();
                configurationRoot.GetSection(GameConnectionOptions.GameConnection)
                                 .Bind(options);
                services
                .AddTransient<IGameConnection>(sp => new GameConnection(options.address, options.port))
                .AddTransient<SC2Process>();
            });



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
