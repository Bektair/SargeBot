using SargeBot.Features.GameData;
using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = SC2APIProtocol.Action;

namespace SargeBot.Features.Macro.ProductionQueue
{
  internal class Producer
  {

    StaticGameData staticGameData;
    public Producer(StaticGameData gameData)
    {
      staticGameData = gameData;

    }

    public Action makeUnit(UnitType unit)
    {
      //Update a list every 10 frames or so?
      //I have a list that takes the unit type and gets the producer
      PlainUnit plainUnit = staticGameData.PlainUnits[unit];
      UnitType produceBuilding = staticGameData.UnitToProducer[unit];
      PlainUnit plainBuilding = staticGameData.PlainUnits[produceBuilding];


      /*      foreach (var unit in observation.Observation.RawData.Units)
            {
              if (unit.Alliance != Alliance.Self)
                continue;

              if (!unit.UnitType.Is(UnitType.ZERG_DRONE))
                continue;

              var command = new ActionRawUnitCommand();
              command.UnitTags.Add(unit.Tag);
              command.AbilityId = (int)Ability.BUILD_SPAWNINGPOOL;
              command.TargetWorldSpacePos = _zergBuildingPlacement.FindPlacement();

              return new() { ActionRaw = new() { UnitCommand = command;
            }*/
      return new() { ActionRaw = new() { } };

    }
  }
}