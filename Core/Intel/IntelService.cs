using Core.Data;
using Core.Extensions;
using Google.Protobuf.Collections;
using SC2APIProtocol;
using SC2ClientApi;

namespace Core.Intel;

public abstract class IntelService : IIntelService
{
    private readonly IDataService _dataService;

    public Dictionary<ulong, IntelUnit> Workers = new(),
        Structures = new(),
        Units = new(),
        EnemyUnits = new(),
        Destructibles = new(),
        XelNagaTowers = new(),
        MineralFields = new(),
        VespeneGeysers = new();

    public IntelService(IEnumerable<IDataService> dataServices)
    {
        _dataService = dataServices.First(x => x.Race == Race);
    }

    //TODO: remove or internal
    public Observation Observation { get; set; } = new();
    public abstract Race Race { get; }

    public List<IntelColony> Colonies { get; set; } = new();

    public List<IntelUnit> GetUnits(UnitType unitType)
    {
        return Units.Values.Where(x => x.UnitType.Is(unitType)).ToList();
    }

    public List<IntelUnit> GetStructures(UnitType unitType)
    {
        return Structures.Values.Where(x => x.UnitType.Is(unitType)).ToList();
    }

    public List<IntelUnit> GetWorkers()
    {
        return Workers.Values.ToList();
    }

    public List<IntelColony> EnemyColonies { get; set; } = new();

    public void OnStart(ResponseObservation firstObservation, ResponseData? responseData = null, ResponseGameInfo? gameInfo = null)
    {
        if (gameInfo != null)
            EnemyColonies.Add(new IntelColony { Point = gameInfo.StartRaw.StartLocations.Last() });

        OnFrame(firstObservation);

        Colonies.Add(new IntelColony { Point = Structures.First().Value.Point });
    }

    public virtual void OnFrame(ResponseObservation observation)
    {
        Observation = observation.Observation;

        HandleUnits(observation.Observation.RawData.Units);

        HandleDeadUnits(observation.Observation.RawData.Event);
    }

    private void HandleDeadUnits(Event? rawDataEvent)
    {
        if (rawDataEvent == null) return;

        foreach (var deadUnit in rawDataEvent.DeadUnits)
            if (Workers.TryGetValue(deadUnit, out var worker))
                Log.Error($"{(UnitType)worker.UnitType} died (tag:{deadUnit})");
            else if (EnemyUnits.TryGetValue(deadUnit, out var enemyUnit))
                Log.Success($"Enemy {(UnitType)enemyUnit.UnitType} died (tag:{deadUnit})");
            else
                Log.Info($"Unknown unit died (tag:{deadUnit})");
    }

    private void HandleUnits(RepeatedField<Unit> rawDataUnits)
    {
        foreach (var unit in rawDataUnits)
            switch (unit.Alliance)
            {
                case Alliance.Self:
                    AddUnit(unit);
                    break;
                case Alliance.Enemy:
                    AddEnemyUnit(unit);
                    break;
                case Alliance.Neutral:
                    break;
                case Alliance.Ally:
                default:
                    throw new NotImplementedException();
            }
    }

    private void AddEnemyUnit(Unit unit)
    {
        AddOrUpdateIntelUnits(EnemyUnits, unit);
    }

    private void AddUnit(Unit unit)
    {
        if (unit.UnitType.IsWorker())
            AddOrUpdateIntelUnits(Workers, unit);
        else if (_dataService.IsStructure(unit.UnitType))
            AddOrUpdateIntelUnits(Structures, unit);
        else
            AddOrUpdateIntelUnits(Units, unit);
    }

    private void AddOrUpdateIntelUnits(Dictionary<ulong, IntelUnit> intelUnits, Unit unit)
    {
        if (intelUnits.ContainsKey(unit.Tag))
            intelUnits[unit.Tag].Data = unit;
        else
            intelUnits.Add(unit.Tag, new IntelUnit(unit));
    }
}

public interface IIntelService
{
    public Race Race { get; }
    public List<IntelColony> EnemyColonies { get; set; }
    public List<IntelColony> Colonies { get; set; }

    public List<IntelUnit> GetUnits(UnitType unitType);
    public List<IntelUnit> GetStructures(UnitType unitType);
    public List<IntelUnit> GetWorkers();
    public void OnStart(ResponseObservation firstObservation, ResponseData? responseData = null, ResponseGameInfo? gameInfo = null);
    public void OnFrame(ResponseObservation observation);
}