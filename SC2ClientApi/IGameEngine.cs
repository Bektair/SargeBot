using SC2APIProtocol;
using Action = SC2APIProtocol.Action;

namespace SC2ClientApi;

public interface IGameEngine
{
    void OnStart(string dataFileName = "", ResponseGameInfo? gameInfo = null, ResponseData? responseData = null);
    (List<Action>, List<DebugCommand>) OnFrame(ResponseObservation observation);
}