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

    Task CreateGame(String mapPath, Race opponentRace, Difficulty opponentDifficulty, AIBuild aIBuild, int randomSeed = -1);

    Task<Response> sendJoinGameRequest(Race race);

    Task Run(object bot, uint playerId, string opponentID);

    Task<Response> sendStepRequest();

    Task<Response> sendObservationRequest();

    Task<Response> sendActionsRequest(List<SC2APIProtocol.Action> actions);

    int getPort();

    string getAddress();


}

