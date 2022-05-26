using System.Diagnostics;

namespace SargeBot.GameClient
{
    public interface ISC2Process
    {
        IGameConnection GetGameConnection();
        string getMapPath(string mapName);
        Process? GetProcess();
        void Start();
    }
}