using System.Diagnostics;
using Microsoft.Extensions.Options;
using SargeBot.Features.Debug;
using SargeBot.Features.Macro;
using SargeBot.Options;
using SC2ClientApi;
using Action = SC2APIProtocol.Action;

namespace SargeBot.GameClient;

public class GameEngine
{
    private readonly DebugService _debugService;
    private readonly SC2ClientApi.GameClient _gameClient;
    private readonly MacroManager _macroManager;

    public GameEngine(SC2ClientApi.GameClient gameClient, IOptions<RequestOptions> options, DebugService debugService, MacroManager macroManager)
    {
        _gameClient = gameClient;
        _debugService = debugService;
        _macroManager = macroManager;
    }

    public async Task RunSinglePlayer()
    {
        await _gameClient.Initialize(true);
        await _gameClient.CreateGameRequest();
        await _gameClient.JoinGameRequest();
        await GameLoop();
    }

    public async Task GameLoop()
    {
        var start = true;
        var frames = 0;
        var actionCount = 0;


#if DEBUG
        var totalTimeWatch = new Stopwatch();
        totalTimeWatch.Start();
        var loopTimeWatch = new Stopwatch();
#endif

        while (true)
        {
#if DEBUG
            loopTimeWatch.Restart();
            await _debugService.DrawText($"Elapsed time {totalTimeWatch.Elapsed:g}");
#endif

            if (!start) await _gameClient.StepRequest();

            var response = await _gameClient.SendAndReceive(ClientConstants.RequestObservation);

            var observation = response.Observation;

            await _macroManager.BuildProbe(observation);

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

            var filteredActions = new List<Action>();
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
                // await gameConnection.SendActionsRequest(filteredActions);
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