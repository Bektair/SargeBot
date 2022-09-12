using Core.Data;
using Core.Intel;
using Core.Macro;
using Core.SharedModels;
using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Zerg;

public class ZergBuildingPlacement : IZergBuildingPlacement
{
  public readonly IIntelService IntelService;
  public readonly IDataService DataService;
  public Race race = Race.Zerg;


  public ZergBuildingPlacement(IEnumerable<IIntelService> intelServices, IDataService dataService)
  {
    IntelService = intelServices.First(x => x.Race == race);
    DataService = dataService;
  }



  public ulong? FindPlacementGas()
  {
    var myMain = IntelService.Colonies.FirstOrDefault(c=>c.IsStartingLocation);
    if (myMain == null) return null;
    IPosition main = new Position(myMain.Point);

    Console.WriteLine(myMain.Point);

    //How do I check if the baselocations are occupied
    //public List<BaseLocation> BaseLocations { get; set; }
    //public List<BaseLocation> SelfBases { get; set; }

    //zergBuildAbillities.Contains(order.AbilityId)

    var closestGeysirs = IntelService.GetVespeneGeysers().OrderBy(gas=> main.Point.Distance(gas.Point));
  IEnumerable<Point> allExtractors = IntelService.GetUnits(UnitType.ZERG_EXTRACTOR).Select(ex => ex.Pos);
  var BuildDroneOrders = IntelService.GetUnits(UnitType.ZERG_DRONE)
    .Select(builder => builder.Orders.FirstOrDefault(order => order.AbilityId.IsWorkerBuildAbillity()));
  if (BuildDroneOrders == null) return null;
    
    var DroneTargets = BuildDroneOrders.Where(orders => orders != null).Select(orders => orders.TargetUnitTag);

    var closestOpenGeysir = closestGeysirs.FirstOrDefault(c=>!allExtractors.Contains(c.Pos) && !DroneTargets.Contains(c.Tag));
    if (closestOpenGeysir != null)
    {
      Console.WriteLine(closestOpenGeysir.Point);
      return closestOpenGeysir.Tag;
    }
    return null;
  }

  public Point2D? FindPlacement(UnitType unitType, int size)
  {

    var myMain = IntelService.Colonies.FirstOrDefault(c => c.IsStartingLocation);
    if (myMain == null) return null;

    // temporary random placement
    var r = new Random();
    var x = myMain.Point.X + r.Next(-10, 10);
    var y = myMain.Point.Y + r.Next(-10, 10);

    return new Point2D { X = x, Y = y };
  }
}

public interface IZergBuildingPlacement : IBuildingPlacement
{
  /// <summary>
  /// Gas needs to be made using the target neutral unit
  /// </summary>
  /// <returns></returns>
  public ulong? FindPlacementGas();
  public Point2D? FindPlacement(UnitType unitType, int size);

}