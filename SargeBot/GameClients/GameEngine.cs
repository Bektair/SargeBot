using System.Diagnostics;
using Microsoft.Extensions.Options;
using SargeBot.Features.Debug;
using SargeBot.Features.GameData;
using SargeBot.Features.GameInfo;
using SargeBot.Features.Macro;
using SargeBot.Options;
using SC2APIProtocol;
using SC2ClientApi;
using SC2ClientApi.Constants;
using Action = SC2APIProtocol.Action;

namespace SargeBot.GameClients;

public class GameEngine
{
    private readonly DebugService _debugService;
    private readonly GameClient _gameClient;
    private readonly MacroManager _macroManager;
    private readonly MapService _mapService;
    private readonly DataRequestManager _dataRequestManager;

    public GameEngine(GameClient gameClient, IOptions<RequestOptions> options, DebugService debugService, 
        MacroManager macroManager, MapService mapService, DataRequestManager dataRequestManager)
    {
        _gameClient = gameClient;
        _debugService = debugService;
        _macroManager = macroManager;
        _mapService = mapService;
        _dataRequestManager = dataRequestManager;
    }

    public async Task RunSinglePlayer()
    {
        await _gameClient.Initialize();
        await _gameClient.CreateGameRequest();
        var Response = await _gameClient.JoinGameRequest();
        var gameInfoResponse = await _gameClient.GameInfoRequest();
        //if(gameInfoResponse != null && gameInfoResponse.GameInfo !=null) _mapService.PopulateMapData(gameInfoResponse);
        //await _dataRequestManager.LoadData();
        await GameLoop(Response);
    }

    public async Task GameLoop(Response joinResponse)
    {
        var start = true;
        var frames = 0;
        var actionCount = 0;


#if DEBUG
        var totalTimeWatch = new Stopwatch();
        totalTimeWatch.Start();
        var loopTimeWatch = new Stopwatch();
#endif
        var response = new Response();
        do
        {
#if DEBUG
            loopTimeWatch.Restart();
#endif

            if (!start) await _gameClient.StepRequest();
            //Gets all GameState
            response = await _gameClient.SendAndReceive(ClientConstants.RequestObservation);
            var observation = response.Observation;

            if (observation == null) //No gamestate
            {
                if (response.Status == Status.Ended || response.Status == Status.Quit)
                {
                    //     bot.OnEnd(observation, observation.PlayerResult[(int)playerId - 1].Result);
                    break;
                }
            }
            else
            { //Have gamestate
                await _macroManager.BuildPylon(observation);
                await _macroManager.BuildGateWay(observation);
                await _macroManager.BuildCyber(observation);
                await _macroManager.BuildStargate(observation);
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
        } while (response.Status == Status.InGame);
    }
}