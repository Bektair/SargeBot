using Google.Protobuf.Collections;
using SC2APIProtocol;
using static SC2APIProtocol.AbilityData.Types;

namespace SargeBot.Features.GameData;
/// <summary>
/// A more narrow version of AbilityData, ommitting useless fields
/// </summary>
public class Ability
{
    uint AbillityId { get; set; } = 0;
    bool Available { get; set; } = false;
    float CastRange { get; set; } = 0;
    float FootprintRadius { get; set; } = 0;
    string FriendlyName { get; set; } = "";
    bool IsBuilding { get; set; } = false;
    bool IsInstantPlacement { get; set; } = false;
    bool AutoCast { get; set; } = false;
    Target Target { get; set; } = Target.None;

    public Ability(AbilityData abillities)
    {
        //Finally
        AbillityId = abillities.AbilityId;
        Available = abillities.Available;
        CastRange = abillities.CastRange;
        FootprintRadius = abillities.FootprintRadius;
        FriendlyName = abillities.FriendlyName;
        IsBuilding = abillities.IsBuilding;
        IsInstantPlacement = abillities.IsInstantPlacement;
        Target = abillities.Target;
        AutoCast = abillities.AllowAutocast;
    }


}
