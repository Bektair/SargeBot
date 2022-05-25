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
                .AddSingleton<IGameConnection>(sp => new GameConnection(options.address, options.port))
                .AddSingleton<SC2Process>();
            });



var process = ActivatorUtilities.CreateInstance<SC2Process>(host.Services);
var mapPath = process.getMapPath("HardwireAIE.SC2Map");

PlayerSetup opponentPlayer = new() { Race = Race.Protoss, AiBuild = AIBuild.Rush, Difficulty = Difficulty.CheatInsane };
var engine = ActivatorUtilities.CreateInstance<GameEngine>(host.Services);
await engine.RunSinglePlayer(mapPath, Race.Protoss, opponentPlayer);


await host.RunAsync();


Console.WriteLine("Hello, World!");