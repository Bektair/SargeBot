// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Net.WebSockets;
using SargeBot;
using SC2APIProtocol;
using SargeBot.GameClient;

Console.WriteLine("Hello, World!");

string address = "127.0.0.1";
int port = 5678;
IGameConnection gameConnection = new GameConnection(address, port);
SC2Process process = new SC2Process(gameConnection);
await (gameConnection.Connect());


var map = "HardwireAIE.SC2Map";
var opponentRace = Race.Protoss;
var opponentDifficulty = Difficulty.Easy;
var aIBuild = AIBuild.Air;
var randomSeed = -1;
await gameConnection.CreateGame(process.getMapPath(map), opponentRace, opponentDifficulty, aIBuild, randomSeed);
var playerId = await gameConnection.JoinGame(opponentRace);
await gameConnection.Run(null, playerId, "test");


Console.WriteLine("Hello, World!");
