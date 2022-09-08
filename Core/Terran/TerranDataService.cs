using Core.Data;
using SC2APIProtocol;

namespace Core.Terran;

public class TerranDataService : DataService
{
    public override Race Race => Race.Terran;

    public override void OnStart(ResponseObservation obs, ResponseData data, ResponseGameInfo gameInfo)
    {
        base.OnStart(obs, data, gameInfo);

        foreach (var unitType in data.Units)
        {
            if (TerranDataHelpers.RequiresTechLab.Contains((UnitType)unitType.UnitId))
                unitType.RequireAttached = true;

            if (TerranDataHelpers.TechRequirement.TryGetValue((UnitType)unitType.UnitId, out var techReq))
                unitType.TechRequirement = (uint)techReq;
        }
    }
}