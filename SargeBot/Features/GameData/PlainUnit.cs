using System.Text.Json.Serialization;
using SargeBot.Features.Macro.ProductionQueues;
using SC2APIProtocol;
using Attribute = SC2APIProtocol.Attribute;

namespace SargeBot.Features.GameData;

public class PlainUnit : IProduceable
{

  public PlainUnit()
  {

  }

  public PlainUnit(UnitTypeData unitType)
    {
        AbilityId = unitType.AbilityId;
        Armor = unitType.Armor;
        Attributes.AddRange(unitType.Attributes);
        Available = unitType.Available;
        BuildTime = unitType.BuildTime;
        CargoSize = unitType.CargoSize;
        FoodProvided = unitType.FoodProvided;
        FoodRequired = unitType.FoodRequired;
        MineralCost = unitType.MineralCost;
        VespeneCost = unitType.VespeneCost;
        MovementSpeed = unitType.MovementSpeed;
        Name = unitType.Name;
        Race = unitType.Race;
        SightRange = unitType.SightRange;
        TechAlias.AddRange(unitType.TechAlias);
        TechRequirement = unitType.TechRequirement;
        UnitAlias = unitType.UnitAlias;
        UnitId = unitType.UnitId;
        Weapons.AddRange(unitType.Weapons);
        RequreAttached = unitType.RequireAttached;
    }

    [JsonConstructor]
    public PlainUnit(string name, uint abilityId, float armor, List<Attribute> attributes, bool available, float buildTime, uint cargoSize, float foodProvided, float foodRequired, uint mineralCost, uint vespeneCost, float movementSpeed, Race race,
        float sightRange, List<uint> techAlias, uint techRequirement, uint unitAlias, uint unitId, List<Weapon> weapons, bool requreAttached)
    {
        Name = name;
        AbilityId = abilityId;
        Armor = armor;
        Attributes = attributes;
        Available = available;
        BuildTime = buildTime;
        CargoSize = cargoSize;
        FoodProvided = foodProvided;
        FoodRequired = foodRequired;
        MineralCost = mineralCost;
        VespeneCost = vespeneCost;
        MovementSpeed = movementSpeed;
        Race = race;
        SightRange = sightRange;
        TechAlias = techAlias;
        TechRequirement = techRequirement;
        UnitAlias = unitAlias;
        UnitId = unitId;
        Weapons = weapons;
        RequreAttached = requreAttached;
    }

    public string Name { get; set; } = string.Empty;
    public uint AbilityId { get; set; }
    public float Armor { get; set; }
    public List<Attribute> Attributes { get; set; } = new();
    public bool Available { get; set; }
    public float BuildTime { get; set; }
    public uint CargoSize { get; set; }
    public float FoodProvided { get; set; }
    public float FoodRequired { get; set; }
    public uint MineralCost { get; set; }
    public uint VespeneCost { get; set; }
    public float MovementSpeed { get; set; }
    public Race Race { get; set; } = Race.NoRace;
    public float SightRange { get; set; }
    public List<uint> TechAlias { get; set; } = new();
    public uint TechRequirement { get; set; }
    public uint UnitAlias { get; set; }
    public uint UnitId { get; set; }
    public List<Weapon> Weapons { get; set; } = new();
    public bool RequreAttached { get; set; }




}