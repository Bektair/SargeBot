using SargeBot.Features.GameData;
using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = SC2APIProtocol.Action;

namespace SargeBot.Features.Macro.ProductionQueues;
/// <summary>
/// This queue holds the order of which the other queues should be activated
/// Should be peeked to know when the next item is to be made
/// Then notified to produce the front item when enough resources
/// </summary>
public class ProductionQueue
{

  StaticGameData staticGameData;

  //When a production facility is idle they subscribe to the production queue ready to work work work

  //The queue will then notify the subscribed facilities when a new unit / research is needed

  
  Queue<PlainUpgrade> UpgradeQueue;

  //Order to execute the queues in?
  Queue<IProductionOrder> OrderQueue { get; }
  IUnitProductionQueue UnitQueue;
  IBuildingProductionQueue BuildingQueue;

  public ProductionQueue(StaticGameData staticGameData, IUnitProductionQueue UnitQueue, IBuildingProductionQueue BuildingQueue)
  {
    //It should accept UnitType, Ability or upgrade
    this.staticGameData = staticGameData;
    OrderQueue = new Queue<IProductionOrder>();
    this.UnitQueue = UnitQueue;
    this.BuildingQueue = BuildingQueue;
  }

  public bool ContainsUnit(UnitType unitType)
  {
    return UnitQueue.Contains(unitType);
  }

  public int CountInstancesOfUnit(UnitType unitType)
  {
    return UnitQueue.CountOfUnitType(unitType);
  }


  public IProductionOrder Peek()
  {
    return OrderQueue.Peek();
  }

  public bool IsEmpty()
  {
    return OrderQueue.Count == 0;
  }

  /// <summary>
  /// Makes a ProductionOrder and Queues it to the production queue
  /// 
  /// </summary>
  /// <param name="unitType"></param>
  public void EnqueueUnit(UnitType unitType)
  {
    PlainUnit unit = staticGameData.PlainUnits[unitType];

    ProductionOrder order = new(orderType: ProductionOrderType.Structure,
      MineralCost: unit.MineralCost, GasCost: unit.VespeneCost, FoodRequired: unit.FoodRequired
      , UnitQueue);
    UnitQueue.Enqueue(unitType);
    OrderQueue.Enqueue(order);
  }
  /// <summary>
  /// Enqueue must have been run at a earlier point must have been queued first
  /// If you are unable to produce it right now it will be added to the pre-productionqueue
  /// Called when you have enough resources to buy it
  /// </summary>
  /// <param name="observation"></param>
  /// <returns></returns>
  public Action ProduceFirstItem(ResponseObservation observation)
  {
    IProductionOrder order = OrderQueue.Dequeue();
    Action? returnAction = order.queue.Activate(observation);
    if (returnAction == null)
    {
      return new() { ActionRaw = new() };
    }
    else { return returnAction; }
  }

  /// <summary>
  /// Forces creation outside the queue itself
  /// </summary>
  /// <param name="observation"></param>
  /// <param name="unitType"></param>
  public Action CreateUnitAction(ResponseObservation obs, UnitType unitType)
  {
    Action? returnAction =  UnitQueue.CreateUnitAction(obs, unitType);
    if (returnAction == null)
    {
      return new() { ActionRaw = new() };
    }
    else { return returnAction; }
  }

  /// <summary>
  /// Tell the queue to build something
  /// </summary>
  /// <param name="buildingType"></param>
  public void EnqueueBuilding(UnitType buildingType)
  {
    //I could queue abillity
    PlainUnit building = staticGameData.PlainUnits[buildingType];
    Ability Ability = (Ability)building.AbilityId;
    IProductionOrder order = new ProductionOrder(orderType: ProductionOrderType.Structure, MineralCost: building.MineralCost, 
      GasCost: building.VespeneCost, FoodRequired: building.FoodRequired, queue: BuildingQueue);
    OrderQueue.Enqueue(order);
    BuildingQueue.Enqueue(Ability);
  }



}
