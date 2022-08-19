

namespace XUnitSargeBot.Tests.Features.Macro.ProductionQueue;

using Google.Protobuf.Collections;
using SargeBot.Features.GameData;
using SargeBot.Features.Macro.ProductionQueue;
using SC2APIProtocol;
using System.Linq;

public class ProductionQueueTest
{
  StaticGameData staticGameData;
  ResponseObservation responseObservation;
  Dictionary<UnitType, Unit> observationUnit = new();

  public ProductionQueueTest()
  {
     staticGameData = new StaticGameData();
    staticGameData.PlainUnits = new(){
      {UnitType.ZERG_QUEEN, new PlainUnit(){ AbilityId = (uint)Ability.TRAIN_QUEEN, MineralCost= 150
      , FoodRequired = 2, VespeneCost = 0, UnitId = (uint)UnitType.ZERG_QUEEN} },
      {UnitType.ZERG_HATCHERY,  new PlainUnit(){ MineralCost=300
      , FoodRequired = 0, FoodProvided = 5, VespeneCost = 0, UnitId = (uint)UnitType.ZERG_QUEEN}}
    };
    staticGameData.PlainAbilities = new()
    {
      {Ability.TRAIN_QUEEN, new PlainAbility(){ AbilityId=(uint)Ability.TRAIN_QUEEN} }
    };

    responseObservation = new ResponseObservation();
    responseObservation.Observation = new Observation();
    responseObservation.Observation.RawData = new ObservationRaw();
    RepeatedField<Unit> units = responseObservation.Observation.RawData.Units;
    units.Add(new Unit() { Alliance = Alliance.Self, Tag = (ulong)UnitType.ZERG_HATCHERY });
    
    foreach(Unit unit in units)
    {
      observationUnit.Add((UnitType)unit.Tag, unit);
    }

  }

  [Fact]
  public void EnqueueUnit_150MineralZERGQUEEN_QueueCosts150Minerals()
  {

    //Both need static game data
    UnitProductionQueue productionQueue = new UnitProductionQueue(staticGameData);
    ProductionQueue queue = new ProductionQueue(staticGameData, productionQueue);
    uint expected = 150;
    queue.EnqueueUnit(UnitType.ZERG_QUEEN);
    //Act
    uint actual = queue.Peek().MineralCost;
    //Assert
    Assert.Equal(expected, actual);

  }

  [Theory]
  [InlineData(UnitType.ZERG_HATCHERY)]
  [InlineData(UnitType.ZERG_LAIR)]
  [InlineData(UnitType.ZERG_HIVE)]
  public void ProduceFirstItem_ZERGQUEEN1Hatchery_ActionToConstruct(UnitType hatcheryLike)
  {
    ResetObsevation();
    AddObservation(new Unit() { Alliance = Alliance.Self, Tag = (ulong)hatcheryLike });

    UnitProductionQueue productionQueue = new UnitProductionQueue(staticGameData);
    ProductionQueue queue = new ProductionQueue(staticGameData, productionQueue);

    var command = new ActionRawUnitCommand();
    command.AbilityId = (int)Ability.TRAIN_QUEEN;
    command.UnitTags.Add(observationUnit[hatcheryLike].Tag);
    Action expected = new() { ActionRaw = new() { UnitCommand = command } };
    queue.EnqueueUnit(UnitType.ZERG_QUEEN);
    //Act
    Action actual = queue.ProduceFirstItem(responseObservation);
    //Assert
    Assert.Equal(expected, actual);

  }

  private void ResetObsevation()
  {
    responseObservation.Observation.RawData.Units.Clear();
    observationUnit.Clear();
  }

  private void AddObservation(Unit unit)
  {
    responseObservation.Observation.RawData.Units.Add(unit);
    observationUnit.Add((UnitType)unit.Tag, unit);
  }

}
