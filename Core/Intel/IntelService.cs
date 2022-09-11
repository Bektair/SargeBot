using Google.Protobuf.Collections;
using SC2APIProtocol;
using SC2ClientApi;
using System.Globalization;
using static Core.Intel.AlgorithmClusterBundle;
using Attribute = SC2APIProtocol.Attribute;

namespace Core.Intel;

public abstract class IntelService : IIntelService
{
    private readonly Dictionary<ulong, IUnit> _allUnits = new();

    private readonly IDataService _dataService;

    public IntelService(IEnumerable<IDataService> dataServices)
    {
        _dataService = dataServices.First(x => x.Race == Race);
    }

    public List<IColony> Colonies { get; } = new();
    public List<IColony> EnemyColonies { get; } = new();
    public List<Point2D> BaseLocations { get;  } = new List<Point2D>();


    public abstract Race Race { get; }
    public uint GameLoop { get; private set; }

    public async void OnStart(ResponseObservation firstObservation, ResponseData? responseData = null, ResponseGameInfo? gameInfo = null, IMessageService? messageService = null)
    {
        if (gameInfo != null)
            EnemyColonies.Add(new IntelColony { Point = gameInfo.StartRaw.StartLocations.Last(), IsStartingLocation = true });


        //Request a query to know the ground distance to each base
        //requestQuery.Pathing
        Point point = firstObservation.Observation.RawData.Units.Where(x => _dataService.HasAttribute(x.UnitType, Attribute.Structure)).Select(x => x.Pos).First();
        Point2D point2d = new Point2D() { X = point.X, Y = point.Y };

        Point2D enemy = EnemyColonies.Last().Point;



        //How do I get Gameconnection
        //    public void Query(Point2D start, Point2D end);
        if (messageService != null)
        {
            //Give up.
            /*      ResponseQuery t = await messageService.Query(point2d, enemy);
                  foreach(var pos in t.Pathing)
                  {
                    Console.Write("distance to enemy main: "+ pos.Distance);
                  }*/
        }



        OnFrame(firstObservation);

        populateBases();

        Colonies.Add(new IntelColony { Point = GetUnits(attribute: Attribute.Structure).First().Point, IsStartingLocation = true });

    }

    // MARKERCLUSTERER config
    // box area i.e. cluster size

    //As the last step in the algorithm all the points are iterated and compared to the existing cluster points. They are assigned to the nearest cluster. This takes O(k*n). The cluster points are located at the center of the boxes.

    private void populateBases()
    {
        var mineralGroups = new Dictionary<uint, int>();
        var minerals = GetMineralFields();
        //https://kunuk.wordpress.com/2011/09/20/markerclusterer-with-c-example-and-html-canvas-part-3/
        //The MarkerClusterer algorithm is simple

        var buckets = new AlgorithmClusterBundle(minerals.Select(unit => new XY(unit.Point.X, unit.Point.Y)).ToList()).markClusterResult.BaseBucketsLookup;
        //Each

        foreach(var bucket in buckets)
        {
            //All the mineralpositions in the bucket, used to calculate baseposition
            Point2D baseLocation = getBaseLocation(
                bucket.Value.Points.Select(xy => new Position((float)xy.X, (float)xy.Y))
            );
            //Console.WriteLine();
            BaseLocations.Add(baseLocation);
        }
    }


    //I am trying to find the center of part of a circle
    //The middle mineralfield go towards
    //Another is to take a bounding box of the mineral line including gas, and taking the middle, offsetting that into open space.
    private Point2D getBaseLocation(IEnumerable<Position> mineralPositions)
    {
        var A = mineralPositions.First().Point;
        mineralPositions = mineralPositions.OrderBy(pos => A.Distance(pos.Point));

        //Furthest away point, as having a larger span of points is important for its correctness.
        var B = mineralPositions.Skip(2).First().Point;
        mineralPositions.GetEnumerator().MoveNext();
        var C = mineralPositions.Last().Point;

        return findCircle(A.X, A.Y, B.X, B.Y, C.X, C.Y);
    }


  
    public int playerMinerals { get; private set; }


    public virtual void OnFrame(ResponseObservation observation)
    {
        GameLoop = observation.Observation.GameLoop;

        HandleUnits(observation.Observation.RawData.Units);

        HandleDeadUnits(observation.Observation.RawData.Event);

        playerMinerals = (int)observation.Observation.PlayerCommon.Minerals;
    }


    public List<IUnit> GetMineralFields() => GetUnitsAllOnMap().Where(x => x.UnitType.IsMineralField()).ToList();

    public List<IUnit> GetVespeneGeysers() => GetUnitsAllOnMap().Where(x => x.UnitType.IsVepeneGeyser()).ToList();

    public List<IUnit> GetDestructibles() => GetUnits(alliance: Alliance.Neutral).Where(x => x.UnitType.IsDestructible()).ToList();

    public List<IUnit> GetXelNagaTowers() => GetUnits(alliance: Alliance.Neutral).Where(x => x.UnitType.IsXelNagaTower()).ToList();

    public List<IUnit> GetUnits(UnitType? unitType = null, Alliance alliance = Alliance.Self, DisplayType displayType = DisplayType.Visible,
        Attribute? attribute = null)
    {
        return _allUnits.Values
            .Where(x => unitType == null || x.UnitType.Is(unitType.Value))
            .Where(x => x.Alliance == alliance)
            .Where(x => x.DisplayType == displayType)
            .Where(x => attribute == null || _dataService.HasAttribute(x.UnitType, attribute.Value))
            .ToList();
    }

    public List<IUnit> GetUnitsAllOnMap()
    {
        return _allUnits.Values
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

    private void AddOrUpdateIntelUnits(Dictionary<ulong, IUnit> intelUnits, Unit unit)
    {
        if (unit.Tag == 0) return; // why does this happen?

        lock (intelUnits)
        {
            if (intelUnits.ContainsKey(unit.Tag))
                intelUnits[unit.Tag].Update(unit, GameLoop);
            else
                intelUnits.Add(unit.Tag, new IntelUnit(unit, GameLoop));
        }
    }


    //https://stackoverflow.com/questions/62488827/solving-equation-to-find-center-point-of-circle-from-3-points
    static Point2D findCircle(double x1, double y1,
                           double x2, double y2,
                           double x3, double y3)
    {
        NumberFormatInfo setPrecision = new NumberFormatInfo();
        setPrecision.NumberDecimalDigits = 3; // 3 digits after the double point

        double x12 = x1 - x2;
        double x13 = x1 - x3;

        double y12 = y1 - y2;
        double y13 = y1 - y3;

        double y31 = y3 - y1;
        double y21 = y2 - y1;

        double x31 = x3 - x1;
        double x21 = x2 - x1;

        double sx13 = (double)(Math.Pow(x1, 2) -
                        Math.Pow(x3, 2));

        double sy13 = (double)(Math.Pow(y1, 2) -
                        Math.Pow(y3, 2));

        double sx21 = (double)(Math.Pow(x2, 2) -
                        Math.Pow(x1, 2));

        double sy21 = (double)(Math.Pow(y2, 2) -
                        Math.Pow(y1, 2));

        double f = ((sx13) * (x12)
                + (sy13) * (x12)
                + (sx21) * (x13)
                + (sy21) * (x13))
                / (2 * ((y31) * (x12) - (y21) * (x13)));
        double g = ((sx13) * (y12)
                + (sy13) * (y12)
                + (sx21) * (y13)
                + (sy21) * (y13))
                / (2 * ((x31) * (y12) - (x21) * (y13)));

        double c = -(double)Math.Pow(x1, 2) - (double)Math.Pow(y1, 2) -
                                    2 * g * x1 - 2 * f * y1;
        double h = -g;
        double k = -f;
        double sqr_of_r = h * h + k * k - c;

        // r is the radius
        double r = Math.Round(Math.Sqrt(sqr_of_r), 5);

        Console.WriteLine("Center of a circle: x = " + h.ToString("N", setPrecision) +
        ", y = " + k.ToString("N", setPrecision));
        Console.WriteLine("Radius: " + r.ToString("N", setPrecision));

        return new Point2D
        {
            X = (float)h,
            Y = (float)k
        };

    }

}



public interface IIntelService
{
    public Race Race { get; }
    public uint GameLoop { get; }
    public int playerMinerals { get;  }

    public List<IColony> Colonies { get; }
    public List<IColony> EnemyColonies { get; }
    public List<Point2D> BaseLocations { get; }

    public List<IUnit> GetUnits(UnitType? unitType = null, Alliance alliance = Alliance.Self, DisplayType displayType = DisplayType.Visible, Attribute? attribute = null);

    public List<IUnit> GetMineralFields();
    public List<IUnit> GetVespeneGeysers();
    public List<IUnit> GetDestructibles();
    public List<IUnit> GetXelNagaTowers();

    public void OnStart(ResponseObservation firstObservation, ResponseData? responseData = null, ResponseGameInfo? gameInfo = null, IMessageService? messageService = null);
    public void OnFrame(ResponseObservation observation);
}