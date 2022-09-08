using Core.Data;
using Core.Extensions;
using Google.Protobuf.Collections;
using SC2APIProtocol;
using SC2ClientApi;

namespace Core.Intel;

public abstract class IntelService : IIntelService
{
    private readonly Dictionary<ulong, IntelUnit>
        _allUnits        = new(),
        _units           = new(),
        _enemyUnits      = new(),
        _workers         = new(),
        _enemyWorkers    = new(),
        _structures      = new(),
        _enemyStructures = new(),
        _destructibles   = new(),
        _xelNagaTowers   = new(),
        _mineralFields   = new(),
        _vespeneGeysers  = new();

    private readonly IDataService _dataService;

    public IntelService(IEnumerable<IDataService> dataServices)
    {
        _dataService = dataServices.First(x => x.Race == Race);
    }

    public abstract Race Race { get; }
    public uint GameLoop { get; private set; }
    public List<IntelColony> Colonies { get; } = new();
    public List<IntelColony> EnemyColonies { get; } = new();

    public List<IntelUnit> GetUnits(UnitType unitType)
    {
        return _units.Values.Where(x => x.UnitType.Is(unitType)).ToList();
    }

    public List<IntelUnit> GetEnemyUnits(UnitType unitType)
    {
        return _enemyUnits.Values.Where(x => x.UnitType.Is(unitType)).ToList();
    }

    public List<IntelUnit> GetStructures(UnitType unitType)
    {
        return _structures.Values.Where(x => x.UnitType.Is(unitType)).ToList();
    }

    public List<IntelUnit> GetEnemyStructures(UnitType unitType)
    {
        return _enemyStructures.Values.Where(x => x.UnitType.Is(unitType)).ToList();
    }

    public List<IntelUnit> GetWorkers()
    {
        return _workers.Values.ToList();
    }

    public List<IntelUnit> GetEnemyWorkers()
    {
        return _enemyWorkers.Values.ToList();
    }

    public List<IntelUnit> GetMineralFields()
    {
        return _mineralFields.Values.ToList();
    }

    public List<IntelUnit> GetVespeneGeysers()
    {
        return _vespeneGeysers.Values.OrderBy(x => x.Point.Distance(Colonies.First().Point)).ToList();
    }

    public List<IntelUnit> GetDestructibles()
    {
        return _destructibles.Values.ToList();
    }

    public List<IntelUnit> GetXelNagaTowers()
    {
        return _xelNagaTowers.Values.ToList();
    }

    public void OnStart(ResponseObservation firstObservation, ResponseData? responseData = null, ResponseGameInfo? gameInfo = null)
    {
        if (gameInfo != null)
            EnemyColonies.Add(new IntelColony { Point = gameInfo.StartRaw.StartLocations.Last() });

        OnFrame(firstObservation);

        Colonies.Add(new IntelColony { Point = _structures.First().Value.Point });
    }

    public virtual void OnFrame(ResponseObservation observation)
    {
        GameLoop = observation.Observation.GameLoop;

        HandleUnits(observation.Observation.RawData.Units);

        HandleDeadUnits(observation.Observation.RawData.Event);
    }

    private void HandleDeadUnits(Event? rawDataEvent)
    {
        if (rawDataEvent == null) return;

        foreach (var deadUnit in rawDataEvent.DeadUnits)
            if (_allUnits.TryGetValue(deadUnit, out var unit))
            {
                if (unit.Alliance == Alliance.Ally)
                {
                    Log.Error($"{(UnitType)unit.UnitType} died (tag:{deadUnit})");
                    //TODO: remove from all dictionaries, or just have the _allUnits and filter on retrieval?
                    _units.Remove(unit.Tag);
                }
                else
                {
                    Log.Success($"Enemy {(UnitType)unit.UnitType} died (tag:{deadUnit})");
                }

                _allUnits.Remove(unit.Tag);
            }
            else
            {
                Log.Info($"Unknown unit died (tag:{deadUnit})");
            }
    }

    private void HandleUnits(RepeatedField<Unit> rawDataUnits)
    {
        foreach (var unit in rawDataUnits)
        {
            AddOrUpdateIntelUnits(_allUnits, unit);
            switch (unit.Alliance)
            {
                case Alliance.Self:
                    AddUnit(unit);
                    break;
                case Alliance.Enemy:
                    AddEnemyUnit(unit);
                    break;
                case Alliance.Neutral:
                    AddNeutralUnit(unit);
                    break;
                case Alliance.Ally:
                default:
                    throw new NotImplementedException();
            }
        }
    }

    private void AddEnemyUnit(Unit unit)
    {
        if (unit.UnitType.IsWorker())
            AddOrUpdateIntelUnits(_enemyWorkers, unit);
        else if (_dataService.IsStructure(unit.UnitType))
            AddOrUpdateIntelUnits(_enemyStructures, unit);
        else
            AddOrUpdateIntelUnits(_enemyUnits, unit);
    }

    private void AddNeutralUnit(Unit unit)
    {
        if (unit.UnitType.IsMineralField())
            AddOrUpdateIntelUnits(_mineralFields, unit);
        else if (unit.UnitType.IsVepeneGeyser())
            AddOrUpdateIntelUnits(_vespeneGeysers, unit);
        else if (unit.UnitType.IsDestructible())
            AddOrUpdateIntelUnits(_destructibles, unit);
        else if (unit.UnitType.IsXelNagaTower())
            AddOrUpdateIntelUnits(_xelNagaTowers, unit);
    }

    private void AddUnit(Unit unit)
    {
        if (unit.UnitType.IsWorker())
            AddOrUpdateIntelUnits(_workers, unit);
        else if (_dataService.IsStructure(unit.UnitType))
            AddOrUpdateIntelUnits(_structures, unit);
        else
            AddOrUpdateIntelUnits(_units, unit);
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
    public uint GameLoop { get; }

    public List<IntelColony> Colonies { get; }
    public List<IntelColony> EnemyColonies { get; }

    public List<IntelUnit> GetUnits(UnitType unitType);
    public List<IntelUnit> GetEnemyUnits(UnitType unitType);
    public List<IntelUnit> GetStructures(UnitType unitType);
    public List<IntelUnit> GetEnemyStructures(UnitType unitType);
    public List<IntelUnit> GetWorkers();
    public List<IntelUnit> GetEnemyWorkers();
    public List<IntelUnit> GetMineralFields();
    public List<IntelUnit> GetVespeneGeysers();
    public List<IntelUnit> GetDestructibles();
    public List<IntelUnit> GetXelNagaTowers();

    public void OnStart(ResponseObservation firstObservation, ResponseData? responseData = null, ResponseGameInfo? gameInfo = null);
    public void OnFrame(ResponseObservation observation);
}