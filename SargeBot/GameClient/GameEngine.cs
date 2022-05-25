using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.GameClient;
public class GameEngine
{
    IGameConnection gameConnection;

    public GameEngine(IGameConnection gameConnection)
    {
        this.gameConnection = gameConnection;
    }


    //AI Difficulty and AI build should be settings
    public async Task RunSinglePlayer(string mapPath, Race myRace, PlayerSetup opponentPlayer, int randomSeed=-1, string opponentID = "test")
    {
        await gameConnection.Connect();
        await gameConnection.CreateGame(mapPath, opponentPlayer.Race, opponentPlayer.Difficulty, opponentPlayer.AiBuild);
        var response = await gameConnection.sendJoinGameRequest(myRace);
        await gameLoop(response.JoinGame.PlayerId, opponentID);
    }


    public async Task gameLoop(uint playerId, string opponentID)
    {
        var start = true;
        int frames = 0;
        int actionCount = 0;

        while (true)
        {

            if (!start)
            {
                await gameConnection.sendStepRequest();
            }
            var response = await gameConnection.sendObservationRequest();


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
                await gameConnection.sendActionsRequest(filteredActions);
                actionCount += filteredActions.Count;
            }
            frames++;
        }
    }



}

