using SC2APIProtocol;
using Action = SC2APIProtocol.Action;

namespace SC2ClientApi;

public interface IGameEngine
{
    void OnStart(ResponseObservation firstObservation, ResponseData responseData, ResponseGameInfo gameInfo);
    (List<Action>, List<DebugCommand>) OnFrame(ResponseObservation observation);
    void OnEnd(ResponseObservation? observation = null);
}