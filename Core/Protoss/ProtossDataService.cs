using Core.Data;
using SC2APIProtocol;
using SC2ClientApi;

namespace Core.Protoss;

public class ProtossDataService : DataService
{
    public override Race Race => Race.Protoss;

    public override void OnStart(ResponseObservation obs, ResponseData? data, ResponseGameInfo? gameInfo)
    {
        base.OnStart(obs, data, gameInfo);

        Log.Info("Toss data");
    }
}