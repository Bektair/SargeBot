using SC2APIProtocol;

namespace SC2ClientApi;

public interface IGameEngine
{
    void OnStart(ResponseGameInfo gameInfo);
    Request OnFrame(ResponseObservation observation);
}