

namespace XUnitSargeBot.Tests.Features.Macro.ProductionQueue;

using Google.Protobuf.Collections;
using SargeBot.Features.GameData;
using SargeBot.Features.Macro.ProductionQueues;
using SC2APIProtocol;
using System.Linq;

public class ProductionQueueTest
{
    StaticGameData staticGameData;
    ResponseObservation responseObservation;
    Dictionary<UnitType, Unit> observationUnit = new();
    UnitProductionQueue productionQueue;
    ProductionQueue queue;
    LarvaQueue larvaQueue;

  RepeatedField<Unit> units;

    public ProductionQueueTest()
    {
     
        staticGameData = new StaticGameData();
        staticGameData.PlainUnits = new()
        {
            {
                UnitType.ZERG_QUEEN,
                new PlainUnit()
                {
                    AbilityId = (uint)Ability.TRAIN_QUEEN,
                    MineralCost = 150
          ,
                    FoodRequired = 2,
                    VespeneCost = 0,
                    UnitId = (uint)UnitType.ZERG_QUEEN
                }
            },
            {
                UnitType.ZERG_ZERGLING,
                new PlainUnit()
                {
                    AbilityId = (uint)Ability.TRAIN_ZERGLING,
                    MineralCost = 50,
                    FoodRequired = 1,
                    VespeneCost = 0,
                    UnitId = (uint)UnitType.ZERG_ZERGLING
                }
            },
            {
                UnitType.ZERG_LARVA,
                new PlainUnit()
                {
                    MineralCost = 0,
                    FoodRequired = 0,
                    VespeneCost = 0,
                    UnitId = (uint)UnitType.ZERG_LARVA
                }
            },
            {
                UnitType.ZERG_HATCHERY,
                new PlainUnit()
                {
                    MineralCost = 300,
                    FoodRequired = 0,
                    FoodProvided = 5,
                    VespeneCost = 0,
                    UnitId = (uint)UnitType.ZERG_HATCHERY
                }
            },
            {
                UnitType.ZERG_ROACH,
                new PlainUnit()
                {
                    MineralCost = 75,
                    FoodRequired = 2,
                    FoodProvided = 0,
                    VespeneCost = 25,
                    UnitId = (uint)UnitType.ZERG_ROACH,
                    AbilityId = (uint)Ability.TRAIN_ROACH,
                }
            },
            {
                UnitType.ZERG_HYDRALISK,
                new PlainUnit()
                {
                    MineralCost = 100,
                    FoodRequired = 2,
                    FoodProvided = 0,
                    VespeneCost = 50,
                    UnitId = (uint)UnitType.ZERG_HYDRALISK,
                    AbilityId = (uint)Ability.TRAIN_HYDRALISK,
                }
            },
            {
                UnitType.ZERG_OVERLORD,
                new PlainUnit()
                {
                    MineralCost = 100,
                    FoodRequired = 0,
                    FoodProvided = 8,
                    VespeneCost = 0,
                    AbilityId = (uint)Ability.TRAIN_OVERLORD,
                    UnitId = (uint)UnitType.ZERG_OVERLORD
                }
            },
        };
        staticGameData.PlainAbilities = new()
        {
            { Ability.TRAIN_QUEEN, new PlainAbility() { AbilityId = (uint)Ability.TRAIN_QUEEN } }
        };
          larvaQueue = new LarvaQueue(staticGameData);
         productionQueue = new UnitProductionQueue(staticGameData, larvaQueue);
         //queue = new ProductionQueue(staticGameData, productionQueue);


        responseObservation = new ResponseObservation();
        responseObservation.Observation = new Observation();
        responseObservation.Observation.RawData = new ObservationRaw();
        units = responseObservation.Observation.RawData.Units;
        //units.Add(new Unit() { Alliance = Alliance.Self, Tag = (ulong)UnitType.ZERG_HATCHERY });

        foreach (Unit unit in units)
        {
            observationUnit.Add((UnitType)unit.Tag, unit);
        }

    }

    [Fact]
    public void EnqueueUnit_150MineralZERGQUEEN_QueueCosts150Minerals()
    {
        //Both need static game data
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
        AddObservation(new Unit() { Alliance = Alliance.Self, Tag = (ulong)hatcheryLike });

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

    [Theory]
    [InlineData(UnitType.ZERG_ZERGLING)]
    [InlineData(UnitType.ZERG_ROACH)]
    [InlineData(UnitType.ZERG_HYDRALISK)]
    public void ProduceFirstItem_CreateBasicUnitFromLarvae_ActionToConstruct(UnitType UnitTypeToCreate)
    {
        UnitType larva = UnitType.ZERG_LARVA;
        AddObservation(new Unit() { Alliance = Alliance.Self, Tag = (ulong)larva });

        PlainUnit UnitToMake = staticGameData.PlainUnits[UnitTypeToCreate];
        var command = new ActionRawUnitCommand();
        command.AbilityId = (int)UnitToMake.AbilityId;
        command.UnitTags.Add(observationUnit[larva].Tag);
        Action expected = new() { ActionRaw = new() { UnitCommand = command } };
        //Act
        queue.EnqueueUnit(UnitTypeToCreate);
        Action actual = queue.ProduceFirstItem(responseObservation);
        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ProduceFirstItem_AddTwoItemsToQueue_FirstItemAddedIsProduced()
    {
        UnitType larva = UnitType.ZERG_LARVA;
        AddObservation(new Unit() { Alliance = Alliance.Self, Tag = (ulong)larva });

        //ArrangeExpected
        var UnitExpectedToMake = staticGameData.PlainUnits[UnitType.ZERG_ROACH];
        var command = new ActionRawUnitCommand();
        command.AbilityId = (int)UnitExpectedToMake.AbilityId;
        command.UnitTags.Add(observationUnit[larva].Tag);
        Action expectedAction = new() { ActionRaw = new() { UnitCommand = command } };
        int expected = expectedAction.ActionRaw.UnitCommand.AbilityId;

        //ACT
        queue.EnqueueUnit(UnitType.ZERG_ROACH);
        queue.EnqueueUnit(UnitType.ZERG_HYDRALISK);
        var actualAction = queue.ProduceFirstItem(responseObservation);
        int actual = actualAction.ActionRaw.UnitCommand.AbilityId;
        //ASSERT
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ProduceFirstItem_AddTwoItemsToQueueProduceTwice_SecoundItemIsProduced()
    {
        UnitType larva = UnitType.ZERG_LARVA;
        AddObservation(new Unit() { Alliance = Alliance.Self, Tag = (ulong)larva });
        UnitType firstUnitQueued = UnitType.ZERG_ROACH;
        UnitType secoundUnitQueued = UnitType.ZERG_HYDRALISK;

        //ArrangeExpected
        var UnitExpectedToMake = staticGameData.PlainUnits[secoundUnitQueued];
        var command = new ActionRawUnitCommand();
        command.AbilityId = (int)UnitExpectedToMake.AbilityId;
        command.UnitTags.Add(observationUnit[larva].Tag);
        Action expectedAction = new() { ActionRaw = new() { UnitCommand = command } };
        int expected = expectedAction.ActionRaw.UnitCommand.AbilityId;

        //ACT
        queue.EnqueueUnit(firstUnitQueued);
        queue.EnqueueUnit(secoundUnitQueued);
        queue.ProduceFirstItem(responseObservation);
        var actualAction = queue.ProduceFirstItem(responseObservation);
        int actual = actualAction.ActionRaw.UnitCommand.AbilityId;
        //ASSERT
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ProduceFirstItem_QueProduceRoachNoLarva_ShouldNotProduce()
    {
        UnitType firstUnitQueued = UnitType.ZERG_ROACH;
        Action expectedAction = new() { ActionRaw = new() };
        //ACT
        queue.EnqueueUnit(firstUnitQueued);
        var actualAction = queue.ProduceFirstItem(responseObservation);
        //ASSERT
        Assert.Equal(expectedAction, actualAction);
    }

  [Fact]
  public void ProduceFirstItem_QueProduceRoachNoLarva_ShouldBe1InLarvaQueue()
  {
    UnitType firstUnitQueued = UnitType.ZERG_ROACH;
    int expected = 1;
    //ACT
    queue.EnqueueUnit(firstUnitQueued);
    queue.ProduceFirstItem(responseObservation);
    int actual = larvaQueue.Count();
    //ASSERT
    Assert.Equal(actual, expected);
  }


  private void AddObservation(Unit unit)
    {
        responseObservation.Observation.RawData.Units.Add(unit);
        observationUnit.Add((UnitType)unit.Tag, unit);
    }

}
