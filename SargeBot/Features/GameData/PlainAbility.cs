using System.Text.Json.Serialization;
using SC2APIProtocol;
using static SC2APIProtocol.AbilityData.Types;

namespace SargeBot.Features.GameData;

/// <summary>
///     A more narrow version of AbilityData, ommitting useless fields
/// </summary>
public class PlainAbility
{
    public PlainAbility(AbilityData abilities)
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
    public PlainAbility()
    {
    }

    // [JsonConstructor]
    // public PlainAbility(string friendlyName, uint abilityId, bool available, float castRange, float footprintRadius, bool isBuilding, bool isInstantPlacement, bool autoCast, Target target)
    // {
    //     FriendlyName = friendlyName;
    //     AbilityId = abilityId;
    //     Available = available;
    //     CastRange = castRange;
    //     FootprintRadius = footprintRadius;
    //     IsBuilding = isBuilding;
    //     IsInstantPlacement = isInstantPlacement;
    //     AutoCast = autoCast;
    //     Target = target;
    // }

    public string FriendlyName { get; set; } = "";
    public uint AbilityId { get; set; }
    public bool Available { get; set; }
    public float CastRange { get; set; }
    public float FootprintRadius { get; set; }
    public bool IsBuilding { get; set; }
    public bool IsInstantPlacement { get; set; }
    public bool AutoCast { get; set; }
    public Target Target { get; set; } = Target.None;
}