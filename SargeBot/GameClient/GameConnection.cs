using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SargeBot.Options;

namespace SargeBot;
public class GameConnection : IGameConnection
{

    private SC2Client sC2Client;
    private readonly string address;
    private readonly int port;

    public GameConnection(IOptions<GameConnectionOptions> options)
    {
        address = options.Value.Address;
        port = options.Value.Port;
        sC2Client = new SC2Client();
    }

    public async Task Connect()
    {
        for (int i = 0; i < 40; i++)
        {
            try
            {
                await sC2Client.Connect(address, port);
                return;
            }
            catch (WebSocketException) { }
            Thread.Sleep(2000);
        }
        throw new Exception("Unable to make a connection.");
    }

    //Should take a playersetup object
    public async Task CreateGame(string mapPath, PlayerSetup opponentPlayer, int randomSeed = -1)
    {
        var createGame = new RequestCreateGame();
        createGame.Realtime = false;

        if (randomSeed >= 0)
        {
            createGame.RandomSeed = (uint)randomSeed;
        }

        createGame.LocalMap = new LocalMap();
        createGame.LocalMap.MapPath = mapPath;

        var player1 = new PlayerSetup();
        createGame.PlayerSetup.Add(player1);
        player1.Type = PlayerType.Participant;

        createGame.PlayerSetup.Add(opponentPlayer);
   

        var request = new Request();
        request.CreateGame = createGame;
        var response = await sC2Client.SendRequest(request);

    }

    public async Task<uint> SendJoinGameRequest(Race race)
    {
        var response = await sC2Client.SendRequest(createJoinGameRequest(race));

        return response.JoinGame.PlayerId;
    }

    private Request CreateJoinGameRequest(Race race)
    {
        var joinGame = new RequestJoinGame();
        joinGame.Race = race;
        joinGame.Options = new InterfaceOptions();
        joinGame.Options.FeatureLayer = new SpatialCameraSetup { CropToPlayableArea = true, AllowCheatingLayers = false, MinimapResolution = new Size2DI { X = 16, Y = 16 }, Resolution = new Size2DI { X = 128, Y = 128 }, Width = 10 };
        joinGame.Options.Raw = true;
        joinGame.Options.Score = true;
        joinGame.Options.ShowCloaked = true;
        joinGame.Options.ShowBurrowedShadows = true;
        joinGame.Options.RawCropToPlayableArea = true;
        joinGame.Options.RawAffectsSelection = true;

        var request = new Request();
        request.JoinGame = joinGame;
        return request;
    }


    public async Task Run(object bot, uint playerId, string opponentID)
    {
        //SC2 API has Data and GameInfo as seperate request types, so I cannot merge these.
        var gameDataRequest = createGameDataRequest();
        var dataResponse = await sC2Client.SendRequest(gameDataRequest);
        var gameInfoReq = createGameInfoRequest();
        var gameInfoResponse = await sC2Client.SendRequest(gameInfoReq);

        var pingRequest = createPingRequest();
        var pingResponse = await sC2Client.SendRequest(pingRequest);

        var observationRequest = createObservationRequest();
        var stepRequest = createStepRequest();




        var start = true;
        int frames = 0;
        int actionCount = 0;


        while (true)
        {

            if (!start)
            {
                await sC2Client.SendRequest(stepRequest);
            }
            var response = await sC2Client.SendRequest(observationRequest);
            

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

            var actionRequest = new Request();
            actionRequest.Action = new RequestAction();
            actionRequest.Action.Actions.AddRange(filteredActions);

            if (actionRequest.Action.Actions.Count > 0)
            {
                await sC2Client.SendRequest(actionRequest);
                actionCount += actionRequest.Action.Actions.Count;
            }
            frames++;
        }
        


    }
    
    private Request CreateStepRequest()
    {
        return new Request
        {
            Step = new RequestStep { Count = 1 }
        };
    }

    private Request CreateObservationRequest()
    {
        return new Request
        {
            Observation = new RequestObservation()
        };
    }

    private Request CreateActionsRequest(List<SC2APIProtocol.Action> actions)
    {
        var actionRequest = new Request();
        actionRequest.Action = new RequestAction();
        actionRequest.Action.Actions.AddRange(actions);
        return actionRequest;
    }

    public  Task<Response> SendStepRequest()
    {
        return sC2Client.SendRequest(createStepRequest());
    }
    public Task<Response> SendObservationRequest()
    {
        return sC2Client.SendRequest(createObservationRequest());
    }

    public Task<Response> SendActionsRequest(List<SC2APIProtocol.Action> actions)
    {
        return sC2Client.SendRequest(createActionsRequest(actions));
    }


    private Request CreateGameDataRequest()
    {
        var gameDataRequest = new Request();
        gameDataRequest.Data = new RequestData();
        gameDataRequest.Data.UnitTypeId = true;
        gameDataRequest.Data.AbilityId = true;
        gameDataRequest.Data.BuffId = true;
        gameDataRequest.Data.EffectId = true;
        gameDataRequest.Data.UpgradeId = true;
        return gameDataRequest;
    }

    private Request CreateGameInfoRequest()
    {
        var gameInfoReq = new Request();
        gameInfoReq.GameInfo = new RequestGameInfo();
        return gameInfoReq;
    }

    private Request CreatePingRequest() { 
        var request = new Request();
        request.Ping = new RequestPing();
        return request;
    }

    public string GetAddress()
    {
        return address;
    }

    public int GetPort()
    {
        return port;
    }


}

