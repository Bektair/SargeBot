﻿using Microsoft.Extensions.FileSystemGlobbing;
using SargeBot.Features.GameData;
using SC2APIProtocol;
using SC2ClientApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = SC2APIProtocol.Action;

namespace SargeBot.Features.Macro.ProductionQueues;
/// <summary>
/// This class is used to produce units
/// </summary>
public class UnitProductionQueue : IUnitProductionQueue
{

  Queue<UnitType> UnitQueue { get; }
  /// <summary>
  /// Waiting for larvae
  /// </summary>
  public LarvaQueue larvaQueue;


  StaticGameData staticGameData;

  public UnitProductionQueue(StaticGameData staticGameData, LarvaQueue larvaQueue)
  {
    UnitQueue = new Queue<UnitType> { };
    this.staticGameData = staticGameData;
    this.larvaQueue = larvaQueue;
  }

  public UnitProductionQueue()
  {
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
      if (producer == UnitType.ZERG_LARVA) { larvaQueue.Enqueue(unitToMake); }

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
    if (larvaQueue.larvaQueue.Contains(unit)) return true;
    return UnitQueue.Contains(unit);
  }

  public int CountOfUnitType(UnitType unit)
  {
    int count = larvaQueue.larvaQueue.Count(u => u == unit);
    return count + UnitQueue.Count(u => u == unit);
  }

  public bool ShouldMakeOverlord(ResponseObservation observation)
  {
    //When supply nears cap force a OV to make and don't run the queue
    //
    if (observation.Observation.PlayerCommon.FoodCap - observation.Observation.PlayerCommon.FoodUsed <= 2)
    {
      int ovInEggs = larvaQueue.EggsOfAbillityId(observation, Ability.TRAIN_OVERLORD);
      if (observation.Observation.PlayerCommon.Minerals >= 100 && ovInEggs == 0 && larvaQueue.CountLarvae(observation) > 0)
      {
        return true;
      }
    }
    return false;
  }
}

