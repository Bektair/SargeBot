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
/// <summary>
/// A type of template, creates request for 
/// That it sends to the SC2Client which communicates directly with API.
/// 
/// </summary>
public class GameConnection : IGameConnection
{

    private SC2Client sC2Client;
    private readonly string address;
    private readonly int port;
    private RequestOptions ReqOptions;

    public GameConnection(IOptions<GameConnectionOptions> ConnOptions, IOptions<RequestOptions> ReqOptions)
    {
        address = ConnOptions.Value.Address;
        port = ConnOptions.Value.Port;
        sC2Client = new SC2Client(port, address);
        this.ReqOptions = ReqOptions.Value;
    }

    public async Task Connect()
    {
        for (int i = 0; i < 40; i++)
        {
            try
            {
                await sC2Client.Connect();
                return;
            }
            catch (WebSocketException) { }
            Thread.Sleep(2000);
        }
        throw new Exception("Unable to make a connection.");
    }

    public async Task CreateGame(string mapPath, PlayerSetup host, PlayerSetup opponentPlayer, int randomSeed = -1)
    {
        var createGame = new RequestCreateGame();
        createGame.Realtime = ReqOptions.Create.Realtime;

        if (randomSeed >= 0)
        {
            createGame.RandomSeed = (uint)randomSeed;
        }
        createGame.LocalMap = new LocalMap();
        createGame.LocalMap.MapPath = mapPath;

        createGame.PlayerSetup.Add(host);
        createGame.PlayerSetup.Add(opponentPlayer);
   

        var request = new Request();
        request.CreateGame = createGame;
        var response = await sC2Client.SendRequest(request);

    }

    public async Task<uint> SendJoinGameRequest()
    {
        var response = await sC2Client.SendRequest(CreateJoinGameRequestHost());

        return response.JoinGame.PlayerId;
    }

    private Request CreateJoinGameRequestHost()
    {
        var joinGame = new RequestJoinGame();
        joinGame.Race = ReqOptions.Host.Race;
        joinGame.Options = ReqOptions.Join;

        var request = new Request();
        request.JoinGame = joinGame;
        return request;
    }


    public async Task Run(object bot, uint playerId, string opponentID)
    {
        //SC2 API has Data and GameInfo as seperate request types, so I cannot merge these.
        var gameDataRequest = CreateGameDataRequest();
        var dataResponse = await sC2Client.SendRequest(gameDataRequest);
        var gameInfoReq = CreateGameInfoRequest();
        var gameInfoResponse = await sC2Client.SendRequest(gameInfoReq);

        var pingRequest = CreatePingRequest();
        var pingResponse = await sC2Client.SendRequest(pingRequest);

        var observationRequest = CreateObservationRequest();
        var stepRequest = CreateStepRequest();




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
        return sC2Client.SendRequest(CreateStepRequest());
    }
    public Task<Response> SendObservationRequest()
    {
        return sC2Client.SendRequest(CreateObservationRequest());
    }

    public Task<Response> SendActionsRequest(List<SC2APIProtocol.Action> actions)
    {
        return sC2Client.SendRequest(CreateActionsRequest(actions));
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

