using Core.Data;
using Core.Extensions;
using Google.Protobuf.Collections;
using SC2APIProtocol;
using SC2ClientApi;
using Attribute = SC2APIProtocol.Attribute;

namespace Core.Intel;

public abstract class IntelService : IIntelService
{
    private readonly Dictionary<ulong, IntelUnit> _allUnits = new();

    private readonly IDataService _dataService;

    public IntelService(IEnumerable<IDataService> dataServices)
    {
        _dataService = dataServices.First(x => x.Race == Race);
    }

    public abstract Race Race { get; }
    public uint GameLoop { get; private set; }
    public List<IntelColony> Colonies { get; } = new();
    public List<IntelColony> EnemyColonies { get; } = new();

    public List<IntelUnit> GetMineralFields() => GetUnits(alliance: Alliance.Neutral).Where(x => x.UnitType.IsMineralField()).ToList();

    public List<IntelUnit> GetVespeneGeysers() => GetUnits(alliance: Alliance.Neutral).Where(x => x.UnitType.IsVepeneGeyser()).ToList();

    public List<IntelUnit> GetDestructibles() => GetUnits(alliance: Alliance.Neutral).Where(x => x.UnitType.IsDestructible()).ToList();

    public List<IntelUnit> GetXelNagaTowers() => GetUnits(alliance: Alliance.Neutral).Where(x => x.UnitType.IsXelNagaTower()).ToList();

    public void OnStart(ResponseObservation firstObservation, ResponseData? responseData = null, ResponseGameInfo? gameInfo = null)
    {
        if (gameInfo != null)
            EnemyColonies.Add(new IntelColony { Point = gameInfo.StartRaw.StartLocations.Last() });

        OnFrame(firstObservation);

        Colonies.Add(new IntelColony { Point = GetUnits(attribute: Attribute.Structure).First().Point, IsStartingLocation=true });
    }

    public virtual void OnFrame(ResponseObservation observation)
    {
        GameLoop = observation.Observation.GameLoop;

        HandleUnits(observation.Observation.RawData.Units);

        HandleDeadUnits(observation.Observation.RawData.Event);
    }

    public List<IntelUnit> GetUnits(UnitType? unitType = null, Alliance alliance = Alliance.Self, DisplayType displayType = DisplayType.Visible,
        Attribute? attribute = null)
    {
        return _allUnits.Values
            .Where(x => unitType == null || x.UnitType.Is(unitType.Value))
            .Where(x => x.Alliance == alliance)
            .Where(x => x.Data.DisplayType == displayType)
            .Where(x => attribute == null || _dataService.HasAttribute(x.UnitType, attribute.Value))
            .ToList();
    }

    private void HandleDeadUnits(Event? rawDataEvent)
    {
        if (rawDataEvent == null) return;

        foreach (var deadUnit in rawDataEvent.DeadUnits)
            if (_allUnits.TryGetValue(deadUnit, out var unit))
            {
                switch (unit.Alliance)
                {
                    case Alliance.Self:
                        Log.Error($"{(UnitType)unit.UnitType} died (tag:{deadUnit})");
                        break;
                    case Alliance.Enemy:
                        Log.Success($"Enemy {(UnitType)unit.UnitType} died (tag:{deadUnit})");
                        break;
                    case Alliance.Neutral:
                        Log.Info($"Neutral {(UnitType)unit.UnitType} died (tag:{deadUnit})");
                        break;
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
        foreach (var unit in rawDataUnits) AddOrUpdateIntelUnits(_allUnits, unit);
    }

    private void AddOrUpdateIntelUnits(Dictionary<ulong, IntelUnit> intelUnits, Unit unit)
    {
        if (unit.Tag == 0) return; // why does this happen?

        lock (intelUnits)
        {
            if (intelUnits.ContainsKey(unit.Tag))
                intelUnits[unit.Tag].Data = unit;
            else
                intelUnits.Add(unit.Tag, new IntelUnit(unit));
        }
    }
}

public interface IIntelService
{
    public Race Race { get; }
    public uint GameLoop { get; }

    public List<IntelColony> Colonies { get; }
    public List<IntelColony> EnemyColonies { get; }

    public List<IntelUnit> GetUnits(UnitType? unitType = null, Alliance alliance = Alliance.Self, DisplayType displayType = DisplayType.Visible, Attribute? attribute = null);

    public List<IntelUnit> GetMineralFields();
    public List<IntelUnit> GetVespeneGeysers();
    public List<IntelUnit> GetDestructibles();
    public List<IntelUnit> GetXelNagaTowers();

    public void OnStart(ResponseObservation firstObservation, ResponseData? responseData = null, ResponseGameInfo? gameInfo = null);
    public void OnFrame(ResponseObservation observation);
}