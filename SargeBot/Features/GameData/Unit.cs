using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.GameData;

public class Unit
{
    uint AbilityId { get; set; } = 0;
    float Armor { get; set; } = 0;
    List<SC2APIProtocol.Attribute> Attributes { get; set; } = new List<SC2APIProtocol.Attribute>();
    bool Available = false;
    float BuildTime { get; set; } = 0;
    uint CargoSize { get; set; } = 0;
    float FoodProvided { get; set; } = 0;
    float FoodRequired { get; set; } = 0;
    uint MineralCost { get; set; } = 0;
    uint VespeneCost { get; set; } = 0;
    float MovementSpeed { get; set; } = 0;
    string Name { get; set; } = string.Empty;
    Race Race { get; set; } = Race.NoRace;
    float SightRange { get; set; } = 0;
    List<uint> TechAlias { get; set; } = new List<uint>();
    uint TechRequirement { get; set; } = 0;
    uint UnitAlias { get; set; } = 0;
    uint UnitId { get; set; } = 0;
    List<Weapon> Weapons { get; set; } = new List<Weapon>();
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

    }




}

