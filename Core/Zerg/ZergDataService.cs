using Core.Data;
using SC2APIProtocol;
using SC2ClientApi;

namespace Core.Zerg;

public class ZergDataService : DataService
{
    public override Race Race => Race.Zerg;

    public override void OnStart(ResponseObservation obs, ResponseData? data, ResponseGameInfo? gameInfo)
    {
        base.OnStart(obs, data, gameInfo);

        Log.Info("Zerg data");
    }
}