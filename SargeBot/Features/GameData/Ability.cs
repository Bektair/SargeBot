using Google.Protobuf.Collections;
using SC2APIProtocol;
using System.Text.Json.Serialization;
using static SC2APIProtocol.AbilityData.Types;

namespace SargeBot.Features.GameData;
/// <summary>
/// A more narrow version of AbilityData, ommitting useless fields
/// </summary>
public class Ability
{
    public string FriendlyName { get; set; } = "";
    public uint AbilityId { get; set; } = 0;
    public bool Available { get; set; } = false;
    public float CastRange { get; set; } = 0;
    public float FootprintRadius { get; set; } = 0;
    public bool IsBuilding { get; set; } = false;
    public bool IsInstantPlacement { get; set; } = false;
    public bool AutoCast { get; set; } = false;
    public Target Target { get; set; } = Target.None;

    public Ability(AbilityData abilities)
    {
        //Finally
        AbilityId = abilities.AbilityId;
        Available = abilities.Available;
        CastRange = abilities.CastRange;
        FootprintRadius = abilities.FootprintRadius;
        FriendlyName = abilities.FriendlyName;
        IsBuilding = abilities.IsBuilding;
        IsInstantPlacement = abilities.IsInstantPlacement;
        Target = abilities.Target;
        AutoCast = abilities.AllowAutocast;
    }

    [JsonConstructor]
    public Ability(string friendlyName, uint abilityId, bool available, float castRange, float footprintRadius, bool isBuilding, bool isInstantPlacement, bool autoCast, Target target)
    {
        FriendlyName = friendlyName;
        AbilityId = abilityId;
        Available = available;
        CastRange = castRange;
        FootprintRadius = footprintRadius;
        IsBuilding = isBuilding;
        IsInstantPlacement = isInstantPlacement;
        AutoCast = autoCast;
        Target = target;
    }
}
