﻿using Core;
using Core.Bot;
using Core.Game;
using Core.Terran;
using Microsoft.Extensions.DependencyInjection;
using SargeBot.Features.GameData;
using SargeBot.Features.GameInfo;
using SargeBot.Features.Macro;
using SargeBot.Features.Macro.Build;
using SargeBot.Features.Macro.Build.PoolRush;
using SargeBot.Features.Macro.Building.Zerg;
using SargeBot.Features.Macro.ProductionQueues;
using SC2APIProtocol;
using SC2ClientApi;

var sp = new ServiceCollection()
    .AddCoreServices()
    .AddScoped<MacroManager>()
    .AddScoped<StaticGameData>()
    .AddScoped<MapDataService>()
    .AddScoped<IntelService>()
    .AddScoped<ZergBuildingPlacement>()
    .AddScoped<ProductionQueue>()
    .AddScoped<IUnitProductionQueue, UnitProductionQueue>()
    .AddScoped<IBuildingProductionQueue, BuildingProductionQueue>()
    .AddScoped<LarvaQueue>()
    .AddScoped<Build>()
    .AddScoped<BuildStateFactory>()
    .BuildServiceProvider();

var gameSettings = new GameSettings(args);
var playerOne = new SargeBot.SargeBot(sp.CreateScope().ServiceProvider);

Game game = gameSettings.GameMode switch
{
    GameMode.Singleplayer => new SingleplayerGame(gameSettings, playerOne, Race.Protoss, AIBuild.Air, Difficulty.Easy),
    GameMode.Ladder => new LadderGame(gameSettings, playerOne),
    GameMode.Multiplayer or _ => new MultiplayerGame(gameSettings, playerOne, new ScvRushBot(sp.CreateScope().ServiceProvider))
};

Log.Info($"Starting {game} {gameSettings}");

await game.Start();