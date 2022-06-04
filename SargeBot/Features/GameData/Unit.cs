using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SargeBot.Features.GameData;

public class Unit
{
    public string Name { get; set; } = string.Empty;
    public uint AbilityId { get; set; } = 0;
    public float Armor { get; set; } = 0;
    public List<SC2APIProtocol.Attribute> Attributes { get; set; } = new List<SC2APIProtocol.Attribute>();
    public bool Available { get; set; } = false;
    public float BuildTime { get; set; } = 0;
    public uint CargoSize { get; set; } = 0;
    public float FoodProvided { get; set; } = 0;
    public float FoodRequired { get; set; } = 0;
    public uint MineralCost { get; set; } = 0;
    public uint VespeneCost { get; set; } = 0;
    public float MovementSpeed { get; set; } = 0;
    public Race Race { get; set; } = Race.NoRace;
    public float SightRange { get; set; } = 0;
    public List<uint> TechAlias { get; set; } = new List<uint>();
    public uint TechRequirement { get; set; } = 0;
    public uint UnitAlias { get; set; } = 0;
    public uint UnitId { get; set; } = 0;
    public List<Weapon> Weapons { get; set; } = new List<Weapon>();
    public bool RequreAttached { get; set; } = false;



    public Unit(UnitTypeData unitType)
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
    public Unit(string name, uint abilityId, float armor, List<SC2APIProtocol.Attribute> attributes, bool available, float buildTime, uint cargoSize, float foodProvided, float foodRequired, uint mineralCost, uint vespeneCost, float movementSpeed, Race race, float sightRange, List<uint> techAlias, uint techRequirement, uint unitAlias, uint unitId, List<Weapon> weapons, bool requreAttached)
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
}

