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
                IHostEnvironment env = hostingcontext.HostingEnvironment;
                configuration
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<GameConnectionOptions>(context.Configuration.GetSection(GameConnectionOptions.GameConnection));
                services.Configure<AIOpponentOptions>(context.Configuration.GetSection(AIOpponentOptions.AIOpponentSettings));
                services
                .AddSingleton<IGameConnection, GameConnection>()
                .AddSingleton<SC2Process>()
                .AddSingleton<GameEngine>();
            });

var process = host.Services.GetRequiredService<SC2Process>();

var aiOptions = host.Services.GetRequiredService<IOptions<AIOpponentOptions>>();




var mapPath = process.getMapPath("HardwireAIE.SC2Map");

var engine = process.gameEngine;

await engine.RunSinglePlayer(mapPath, Race.Protoss);


await host.RunAsync();


Console.WriteLine("Hello, World!");