using Core.Data;
using Core.Intel;
using Google.Protobuf.Collections;
using SC2APIProtocol;

namespace Core.Zerg;

public class ZergIntelService : IntelService, ILarvaService, IOverlordService
{
    public Dictionary<ulong, IntelUnit> Larva = new(), Overlords = new();

    public ZergIntelService(IEnumerable<IDataService> dataServices) : base(dataServices)
    {
    }

    public override Race Race => Race.Zerg;

    public List<IntelUnit> GetLarva()
    {
        return Larva.Values.ToList();
    }

    public List<IntelUnit> GetOverlords()
    {
        return Overlords.Values.ToList();
    }

    public override void OnFrame(ResponseObservation observation)
    {
        base.OnFrame(observation);
        HandleUnits(observation.Observation.RawData.Units);
    }

    private void HandleUnits(RepeatedField<Unit> rawDataUnits)
    {
        foreach (var unit in rawDataUnits)
            switch (unit.Alliance)
            {
                case Alliance.Self:
                    AddZergUnit(unit);
                    break;
                case Alliance.Enemy:
                    break;
                case Alliance.Neutral:
                    break;
                case Alliance.Ally:
                default:
                    throw new NotImplementedException();
            }
    }

    private void AddZergUnit(Unit unit)
    {
        if (unit.UnitType == (int)UnitType.ZERG_LARVA)
        {
            if (Larva.ContainsKey(unit.Tag))
                Larva[unit.Tag].Data = unit;
            else
                Larva.Add(unit.Tag, new IntelUnit(unit));
        }
        else if (unit.UnitType == (int)UnitType.ZERG_OVERLORD)
        {
            if (Overlords.ContainsKey(unit.Tag))
                Overlords[unit.Tag].Data = unit;
            else
                Overlords.Add(unit.Tag, new IntelUnit(unit));
        }
    }
}

public interface ILarvaService
{
    public List<IntelUnit> GetLarva();
}
public interface IOverlordService
{
    public List<IntelUnit> GetOverlords();
}