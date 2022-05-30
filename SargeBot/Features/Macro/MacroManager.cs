using SC2APIProtocol;
using SC2ClientApi.Constants;
using Action = SC2APIProtocol.Action;

namespace SargeBot.Features.Macro;
/// <summary>
/// Builds
/// Trains
/// Upgrades
/// </summary>
public class MacroManager
{
    private readonly SC2ClientApi.GameClient _gameClient;

    public MacroManager(SC2ClientApi.GameClient gameClient)
    {
        _gameClient = gameClient;
    }


    public async Task BuildProbe(ResponseObservation observation)
    {

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

    public async Task BuildPylon(ResponseObservation observation)
    {
        foreach (var unit in observation.Observation.RawData.Units)
        {
            if (unit.Alliance != Alliance.Self)
                continue;

            if (unit.UnitType != (uint)UnitTypes.PROTOSS_PROBE)
                continue;

            var command = new ActionRawUnitCommand();
            command.UnitTags.Add(unit.Tag);
            command.AbilityId = (int)Abilities.BUILD_PYLON;
            command.TargetWorldSpacePos = new Point2D { X=20, Y=30 };


            var action = new Action
            {
                ActionRaw = new()
                {
                    UnitCommand = command
                }
            };

            await SendActionRequests(new() { action });
        }

    }
    public async Task BuildGateWay(ResponseObservation observation)
    {
        foreach (var unit in observation.Observation.RawData.Units)
        {
            if (unit.Alliance != Alliance.Self)
                continue;

            if (unit.UnitType != (uint)UnitTypes.PROTOSS_PROBE)
                continue;

            var command = new ActionRawUnitCommand();
            command.UnitTags.Add(unit.Tag);
            command.AbilityId = (int)Abilities.BUILD_GATEWAY;
            command.TargetWorldSpacePos = new Point2D { X = 22, Y = 32 };


            var action = new Action
            {
                ActionRaw = new()
                {
                    UnitCommand = command
                }
            };

            await SendActionRequests(new() { action });
        }

    }
    public async Task BuildCyber(ResponseObservation observation)
    {
        foreach (var unit in observation.Observation.RawData.Units)
        {
            if (unit.Alliance != Alliance.Self)
                continue;

            if (unit.UnitType != (uint)UnitTypes.PROTOSS_PROBE)
                continue;

            var command = new ActionRawUnitCommand();
            command.UnitTags.Add(unit.Tag);
            command.AbilityId = (int)Abilities.BUILD_CYBERNETICSCORE;
            command.TargetWorldSpacePos = new Point2D { X = 22, Y = 35 };


            var action = new Action
            {
                ActionRaw = new()
                {
                    UnitCommand = command
                }
            };

            await SendActionRequests(new() { action });
        }

    }
    public async Task BuildStargate(ResponseObservation observation)
    {
        foreach (var unit in observation.Observation.RawData.Units)
        {
            if (unit.Alliance != Alliance.Self)
                continue;

            if (unit.UnitType != (uint)UnitTypes.PROTOSS_STARGATE)
                continue;

            var command = new ActionRawUnitCommand();
            command.UnitTags.Add(unit.Tag);
            command.AbilityId = (int)Abilities.BUILD_STARGATE;
            command.TargetWorldSpacePos = new Point2D { X = 19, Y = 32 };


            var action = new Action
            {
                ActionRaw = new()
                {
                    UnitCommand = command
                }
            };

            await SendActionRequests(new() { action });
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