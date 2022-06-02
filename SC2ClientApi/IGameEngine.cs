using SC2APIProtocol;
using Action = SC2APIProtocol.Action;

namespace SC2ClientApi;

public interface IGameEngine
{
    void OnStart(ResponseGameInfo gameInfo);
    List<Action> OnFrame(ResponseObservation observation);
}