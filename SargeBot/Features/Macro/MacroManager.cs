using SC2APIProtocol;
using Action = SC2APIProtocol.Action;

namespace SargeBot.Features.Macro;

public class MacroManager
{
    private readonly SC2Client _sc2Client;

    public MacroManager(SC2Client sc2Client)
    {
        _sc2Client = sc2Client;
    }

    public async Task BuildProbe(ResponseObservation observation)
    {
        var TRAIN_PROBE = 1006;
        var NEXUS = 59;

        foreach (var unit in observation.Observation.RawData.Units)
        {
            if (unit.Alliance != Alliance.Self)
                continue;

            if (unit.UnitType != NEXUS)
                continue;

            var command = new ActionRawUnitCommand();
            command.UnitTags.Add(unit.Tag);
            command.AbilityId = TRAIN_PROBE;

            var action = new Action
            {
                ActionRaw = new()
                {
                    UnitCommand = command
                }
            };

            await SendActionRequests(new() {action});
        }
    }

    private async Task SendActionRequests(List<Action> actions)
    {
        var actionRequest = new Request();
        actionRequest.Action = new();
        actionRequest.Action.Actions.AddRange(actions);

        await _sc2Client.SendRequest(actionRequest);
    }
}