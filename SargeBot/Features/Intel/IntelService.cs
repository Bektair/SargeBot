using SC2APIProtocol;
using SC2ClientApi;

namespace SargeBot.Features.Intel;

public class IntelService
{
    public List<IntelColony> SelfColonies { get; set; } = new();
    public List<IntelColony> EnemyColonies { get; set; } = new();
    public List<Unit> Destructibles { get; set; } = new();
    public List<Unit> XelNagaTowers { get; set; } = new();
    public List<Unit> SelfBuildings { get; set; } = new();
    public List<Unit> StartingMineralFields { get; set; } = new();
    public Point? SelfNatural { get; set; }

    public void OnStart(ResponseObservation firstObservation, ResponseData? responseData = null, ResponseGameInfo? gameInfo = null)
    {
        if (gameInfo != null)
            EnemyColonies.Add(new() {Point = gameInfo.StartRaw.StartLocations.Last()});

        foreach (var unit in firstObservation.Observation.RawData.Units)
            switch (unit.Alliance)
            {
                case Alliance.Self:
                    if (unit.UnitType.IsMainBuilding())
                    {
                        SelfColonies.Add(new() {IsStartingLocation = true, Point = new() {X = unit.Pos.X, Y = unit.Pos.Y}});
                        SelfBuildings.Add(unit);
                    }

                    break;
                case Alliance.Neutral:
                    if (unit.UnitType.Is(UnitType.NEUTRAL_XELNAGATOWER)) XelNagaTowers.Add(unit);
                    if (unit.UnitType.IsDestructible()) Destructibles.Add(unit);
                    if (unit.UnitType.IsMineralField()) StartingMineralFields.Add(unit);
                    break;
            }
    }

    public void OnFrame(ResponseObservation observation)
    {
        // the first discovered minerals not in starting minerals are probably natural, or third
        foreach (var unit in observation.Observation.RawData.Units)
            switch (unit.Alliance)
            {
                case Alliance.Neutral:
                    if (unit.UnitType.IsMineralField() && SelfNatural == null && !StartingMineralFields.Contains(unit))
                        SelfNatural = unit.Pos;
                    break;
            }
    }
}