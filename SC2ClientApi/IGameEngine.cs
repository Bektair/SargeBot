using SC2APIProtocol;
using Action = SC2APIProtocol.Action;

namespace SC2ClientApi;

public interface IGameEngine
{
    void LoadFromCache(string gameMap, bool shouldLoadDataCache, bool shouldLoadInfoCache);
    void OnStart(ResponseObservation firstObservation, ResponseData? responseData = null, ResponseGameInfo? gameInfo = null, string mapName = "");
    (List<Action>, List<DebugCommand>) OnFrame(ResponseObservation observation);
}