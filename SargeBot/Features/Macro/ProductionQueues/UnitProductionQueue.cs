using Microsoft.Extensions.FileSystemGlobbing;
using SargeBot.Features.GameData;
using SC2APIProtocol;
using SC2ClientApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = SC2APIProtocol.Action;

namespace SargeBot.Features.Macro.ProductionQueues
{
  /// <summary>
  /// Concrete subject
  /// </summary>
  public class UnitProductionQueue : IUnitProductionQueue
  {

    Queue<UnitType> UnitQueue { get; }
    /// <summary>
    /// Waiting for larvae
    /// </summary>
    public LarvaQueue LarvaQueue;


    StaticGameData staticGameData;

    public UnitProductionQueue(StaticGameData staticGameData, LarvaQueue larvaQueue)
    {
      UnitQueue = new Queue<UnitType> { };
      this.staticGameData = staticGameData;
      this.LarvaQueue = larvaQueue;
    }

    public UnitProductionQueue()
    {
    }

    public Action? CreateUnitAction(ResponseObservation obs, UnitType unitToMake)
    {
      //If 
      UnitType producer = staticGameData.UnitToProducer[unitToMake];
      //Find the actual unit doing the producing
      Unit? producingBuilding = findProducingBuilding(producer, obs);

      var command = new ActionRawUnitCommand();
      if (producingBuilding == null)
      {
        //If the unit needs larvae update the larvae queue and have the productionqueue get the new state ;)
        if (producer == UnitType.ZERG_LARVA) { LarvaQueue.Enqueue(unitToMake); }

        return null;
      }
      else
      {
        command.UnitTags.Add(producingBuilding.Tag);
        PlainUnit unit = staticGameData.PlainUnits[unitToMake];
        command.AbilityId = (int)unit.AbilityId;
        return new() { ActionRaw = new() { UnitCommand = command } };
      }
    }

    /// <summary>
    /// Tell the subscribers about the newly active entry in the queue
    /// Say we get a order to build a queen which is the only allowed Type atm
    /// Returns null if the producer of the unit is not in the observation list
    /// </summary>
    public Action? Activate(ResponseObservation obs)
    {
      UnitType unitToMake = UnitQueue.Dequeue();
      return CreateUnitAction(obs, unitToMake);
    }

    private Unit? findProducingBuilding(UnitType producer, ResponseObservation obs)
    {
      if (producer == UnitType.ZERG_HATCHERY)
      { return findProducer(obs, staticGameData.hatcheryLike); }
      else if (producer == UnitType.PROTOSS_GATEWAY)
      { return findProducer(obs, staticGameData.gatewayLike); }
      else
      { return findProducer(obs, producer); }
    }

    private Unit? findProducer(ResponseObservation obs, UnitType producer)
    {
      var alleUnits = obs.Observation.RawData.Units;
      IEnumerable<Unit> producers = alleUnits.Where(u => u.UnitType.Is(producer));
      if (producers.Count() > 0) return producers.First();
      else return null;
    }
    private Unit? findProducer(ResponseObservation obs, List<UnitType> producers)
    {
      var alleUnits = obs.Observation.RawData.Units;
        foreach (UnitType producer in producers)
        {
        IEnumerable<Unit> producersEnum = alleUnits.Where(u => u.UnitType.Is(producer));
        if (producersEnum.Count() > 0) return producersEnum.First();
        else return null;
      }
      return null;
    }
    public void Enqueue(UnitType unit)
    {
      UnitQueue.Enqueue(unit);
    }

    public bool Contains(UnitType unit)
    {
      if (LarvaQueue.larvaQueue.Contains(unit)) return true;
      return UnitQueue.Contains(unit);
    }

    public int CountOfUnitType(UnitType unit)
    {
      int count = LarvaQueue.larvaQueue.Where(u => u == unit).Count();
      return count + UnitQueue.Where(u => u == unit).Count();
    }
  }
}

