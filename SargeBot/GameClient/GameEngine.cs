﻿using Microsoft.Extensions.Options;
using SargeBot.Options;
using SC2APIProtocol;
using System.Diagnostics;

namespace SargeBot.GameClient;
/// <summary>
///  
/// </summary>
public class GameEngine
{
    IGameConnection gameConnection;
    private readonly PlayerSetup aiOpponent;
    private readonly PlayerSetup host;
    private string MapPath;

    public GameEngine(IGameConnection gameConnection, IOptions<RequestOptions> options, SC2Process process)
    {
        this.gameConnection = gameConnection;

        MapPath = process.mapPath;

        host = options.Value.Host;
        aiOpponent = options.Value.AIClient;
    }

    public async Task RunSinglePlayer(int randomSeed = -1, string opponentID = "test")
    {
        await gameConnection.Connect();
        await gameConnection.CreateGame(MapPath, host, aiOpponent);
        var playerId = await gameConnection.SendJoinGameRequest();
        await GameLoop(playerId, opponentID);
    }

    public async Task GameLoop(uint playerId, string opponentID)
    {
        var start = true;
        int frames = 0;
        int actionCount = 0;


#if DEBUG
        Stopwatch totalTimeWatch = new Stopwatch();
        totalTimeWatch.Start();
        Stopwatch loopTimeWatch = new Stopwatch();
#endif

        while (true)
        {
#if DEBUG
            loopTimeWatch.Restart();
#endif

            if (!start)
            {
                await gameConnection.SendStepRequest();
            }
            var response = await gameConnection.SendObservationRequest();


            var observation = response.Observation;

            // if (observation == null)
            // {
            //     bot.OnEnd(observation, Result.Undecided);
            //     break;
            // }
            // if (response.Status == Status.Ended || response.Status == Status.Quit)
            // {
            //     bot.OnEnd(observation, observation.PlayerResult[(int)playerId - 1].Result);
            //     break;
            // }

            if (start)
            {
                start = false;
                // bot.OnStart(gameInfoResponse.GameInfo, dataResponse.Data, pingResponse, observation, playerId, opponentID);
            }

            // var actions = bot.OnFrame(observation);

            // var generatedActions = actions.Count();
            // actions = actions.Where(action => action?.ActionRaw?.UnitCommand?.UnitTags == null ||
            //     (action?.ActionRaw?.UnitCommand?.UnitTags != null && 
            //     !action.ActionRaw.UnitCommand.UnitTags.Any(tag => !observation.Observation.RawData.Units.Any(u => u.Tag == tag))));
            // var removedActions = generatedActions - actions.Count();
            // if (removedActions > 0)
            // {
            //     // Console.WriteLine($"Removed {removedActions} actions for units that are not controllable");
            // }

            var filteredActions = new List<SC2APIProtocol.Action>();
            var tags = new List<ulong>();
            // foreach (var action in actions)
            // {
            //     if (action?.ActionRaw?.UnitCommand?.UnitTags != null && !action.ActionRaw.UnitCommand.QueueCommand)
            //     {
            //         if (!tags.Any(tag => action.ActionRaw.UnitCommand.UnitTags.Any(t => t == tag)))
            //         {
            //             filteredActions.Add(action);
            //             tags.AddRange(action.ActionRaw.UnitCommand.UnitTags);
            //         }
            //         else
            //         {
            //             // Console.WriteLine($"{observation.Observation.GameLoop} Removed conflicting order {action.ActionRaw.UnitCommand.AbilityId} for tags {string.Join(" ", action.ActionRaw.UnitCommand.UnitTags)}");
            //         }
            //     }
            //     else
            //     {
            //         filteredActions.Add(action);
            //     }
            // }


            if (filteredActions.Count > 0)
            {
                await gameConnection.SendActionsRequest(filteredActions);
                actionCount += filteredActions.Count;
            }

#if DEBUG
            if (frames == 0) Console.WriteLine("First Loop " + loopTimeWatch.ElapsedMilliseconds);

            if (frames % 100 == 0)
            {
                Console.WriteLine("Total time expired " + totalTimeWatch.Elapsed);
                Console.WriteLine("Time this loop " + loopTimeWatch.ElapsedMilliseconds + "ms");
            }
#endif
            frames++;
        }
    }
}

