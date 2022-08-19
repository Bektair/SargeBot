using SargeBot.Features.GameData;
using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = SC2APIProtocol.Action;

namespace SargeBot.Features.Macro.ProductionQueue;
/// <summary>
/// This is the concrete subject
/// </summary>
public class ProductionQueue
{

  StaticGameData staticGameData;

  //When a production facility is idle they subscribe to the production queue ready to work work work

  //The queue will then notify the subscribed facilities when a new unit / research is needed

  Queue<PlainAbility> BuildingQueue;
  Queue<PlainUpgrade> UpgradeQueue;

  //Order to execute the queues in?
  Queue<IProductionOrder> OrderQueue { get; }
  IUnitProductionQueue UnitQueue;


  public ProductionQueue(StaticGameData staticGameData, IUnitProductionQueue UnitQueue)
  {
    //It should accept UnitType, Ability or upgrade
    this.staticGameData = staticGameData;
    OrderQueue = new Queue<IProductionOrder>();
    this.UnitQueue = UnitQueue;
  }

  public IProductionOrder Peek()
  {
    return OrderQueue.Peek();
  }

  /// <summary>
  /// Makes a ProductionOrder and Queues it to the production queue
  /// 
  /// </summary>
  /// <param name="unitType"></param>
  public void EnqueueUnit(UnitType unitType)
  {
    UnitQueue.Enqueue(unitType);
    PlainUnit unit = staticGameData.PlainUnits[unitType];

    ProductionOrder order = new(orderType: ProductionOrderType.Structure, 
      MineralCost: unit.MineralCost, GasCost: unit.VespeneCost, FoodRequired: unit.FoodRequired
      , UnitQueue);
    OrderQueue.Enqueue(order);

  }
  /// <summary>
  /// Enqueue must have been run at a earlier point must have been queued first
  /// </summary>
  /// <param name="observation"></param>
  /// <returns></returns>
  public Action ProduceFirstItem(ResponseObservation observation)
  {
    return OrderQueue.Dequeue().queue.Activate(observation);
  }

  /// <summary>
  /// Makes a production order of IProductionOrder
  /// </summary>
  /// <param name="item"></param>


  /// <summary>
  /// Tell the queue to build something
  /// </summary>
  /// <param name="building"></param>
/*  public void EnqueueBuilding(UnitType building)
  {
    //I could queue abillity 
    PlainUnit unit = staticGameData.PlainUnits[building];
    PlainAbility plainAbility = staticGameData.PlainAbilities[UnitToAbility(unit)];

    IProductionOrder order = new ProductionOrder(orderType: ProductionOrderType.Structure, MineralCost: unit.MineralCost, GasCost: unit.VespeneCost, FoodRequired: unit.FoodRequired);
    OrderQueue.Enqueue(order);
    BuildingQueue.Enqueue(plainAbility);

    if (OrderQueue.Count == 0)
      QueueIsUpdated();

  }*/

  /// <summary>
  /// Called when you have enough minerals to buy the unit in front
  /// The caller needs to remember it has called it allready by storing reference
  /// </summary>
  /// <returns></returns>


}
