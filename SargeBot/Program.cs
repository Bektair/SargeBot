﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SargeBot.Features.Debug;
using SargeBot.Features.GameData;
using SargeBot.Features.GameInfo;
using SargeBot.Features.Macro;
using SargeBot.GameClients;
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
        services.Configure<RequestOptions>(context.Configuration.GetSection(RequestOptions.RequestSettings));
        services.Configure<ProcessOptions>(context.Configuration.GetSection(ProcessOptions.ProcessSettings));
        services
            .AddSingleton(x => new GameClient(x.CreateGameSettings()))
            .AddSingleton<GameEngine>()
            .AddSingleton<DebugService>()
            .AddSingleton<MacroManager>()
            .AddSingleton<ProcessOptions>()
            .AddSingleton<MapData>()
            .AddSingleton<MapService>()
            .AddSingleton<DataRequestManager>()
            .AddSingleton<GameData>()
            ;
    }).Build();

var gameEngine = host.Services.GetRequiredService<GameEngine>();

// if args.length > 0 run ladder
await gameEngine.RunSinglePlayer();

await host.RunAsync();