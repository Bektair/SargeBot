using Core;
using Core.Game;
using Core.Protoss;
using Core.Zerg;
using Microsoft.Extensions.DependencyInjection;
using SC2APIProtocol;
using SC2ClientApi;

var sp = new ServiceCollection()
    .AddCoreServices()
    .BuildServiceProvider();

var gameSettings = new GameSettings(args);
var playerOne = new DroneRushBot(sp.CreateScope().ServiceProvider);

Game game = gameSettings.GameMode switch
{
    GameMode.Singleplayer => new SingleplayerGame(gameSettings, playerOne, Race.Protoss, AIBuild.Air, Difficulty.Easy),
    GameMode.Ladder => new LadderGame(gameSettings, playerOne),
    GameMode.Multiplayer or _ => new MultiplayerGame(gameSettings, playerOne, new ProbeRushBot(sp.CreateScope().ServiceProvider))
};

Log.Info($"Starting {game} {gameSettings}");

await game.Start();