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

namespace SargeBot.Features.Macro.ProductionQueue
{
  /// <summary>
  /// Concrete subject
  /// </summary>
  public class UnitProductionQueue : IUnitProductionQueue
  {

    Queue<UnitType> UnitQueue { get; }

    StaticGameData staticGameData;

    public UnitProductionQueue(StaticGameData staticGameData)
    {
      UnitQueue = new Queue<UnitType> { };
      this.staticGameData = staticGameData;
    }

    /// <summary>
    /// Tell the subscribers about the newly active entry in the queue
    /// Say we get a order to build a queen which is the only allowed Type atm
    /// </summary>
    public Action Activate(ResponseObservation obs)
    {
      UnitType unitToMake = UnitQueue.Dequeue();
      Unit? producingBuilding = null;
      //If 
      UnitType producer = staticGameData.UnitToProducer[unitToMake];
      var command = new ActionRawUnitCommand();

      //The off case would be hatchery and gateway as there would be multiple producers
      if (producer == UnitType.ZERG_HATCHERY) 
      {producingBuilding=findProducer(obs, staticGameData.hatcheryLike);}
      else if (producer == UnitType.PROTOSS_GATEWAY) 
      { findProducer(obs, staticGameData.gatewayLike); }
      else
      {producingBuilding = findProducer(obs, producer);}
      //A command is the unit to perform it and the abillity to be performed
      command.UnitTags.Add(producingBuilding.Tag);
      PlainUnit unit = staticGameData.PlainUnits[unitToMake];
      command.AbilityId = (int)unit.AbilityId;
      return new() { ActionRaw = new() { UnitCommand = command } };
      
    }

    private Unit? findProducer(ResponseObservation obs, UnitType producer)
    {
      var alleUnits = obs.Observation.RawData.Units;

      foreach (Unit currentUnit in alleUnits)
      {
        if (currentUnit.Tag == (ulong)producer)
        {
          return currentUnit;
        }
      }
      return null;
    }
    private Unit? findProducer(ResponseObservation obs, List<UnitType> producer)
    {
      var alleUnits = obs.Observation.RawData.Units;

      foreach (Unit currentUnit in alleUnits)
      {
        foreach(UnitType type in producer)
        {
          if ((ulong)type == currentUnit.Tag)
          {
            return currentUnit;
          }
        }
      }
      return null;

    }
      public void Enqueue(UnitType unit)
    {
      UnitQueue.Enqueue(unit);
    }

    
  }
}

