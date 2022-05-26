using SC2APIProtocol;

namespace SargeBot.GameClient
{
    public interface IGameEngine
    {
        Task gameLoop(uint playerId, string opponentID);
        Task RunSinglePlayer(string mapPath, Race myRace, PlayerSetup opponentPlayer, int randomSeed = -1, string opponentID = "test");
    }
}