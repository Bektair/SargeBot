using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot;
public interface IGameConnection
{

    Task Connect();

    Task CreateGame(String mapPath, PlayerSetup player, int randomSeed = -1);

    Task<uint> SendJoinGameRequest(Race race);

    Task Run(object bot, uint playerId, string opponentID);

    Task<Response> SendStepRequest();

    Task<Response> SendObservationRequest();

    Task<Response> SendActionsRequest(List<SC2APIProtocol.Action> actions);

    int GetPort();

    string GetAddress();


}

