using SC2APIProtocol;
using SC2ClientApi.Constants;
using Action = SC2APIProtocol.Action;

namespace SargeBot.Features.Macro;

public class MacroManager
{
    private readonly SC2ClientApi.GameClient _gameClient;

    public MacroManager(SC2ClientApi.GameClient gameClient)
    {
        _gameClient = gameClient;
    }


    public async Task BuildProbe(ResponseObservation? observation)
    {
        if (observation == null) return;

        foreach (var unit in observation.Observation.RawData.Units)
        {
            if (unit.Alliance != Alliance.Self)
                continue;

            if (unit.UnitType != (uint)UnitTypes.PROTOSS_NEXUS)
                continue;

            var command = new ActionRawUnitCommand();
            command.UnitTags.Add(unit.Tag);
            command.AbilityId = (int)Abilities.TRAIN_PROBE;

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

        await _gameClient.SendRequest(actionRequest);
    }
}