using SC2APIProtocol;
using SC2ClientApi;
using SC2ClientApi.Constants;
using Action = SC2APIProtocol.Action;

namespace SargeBot.GameClients;

public class GameEngineTwo : IGameEngine
{
    public void OnStart(ResponseGameInfo gameInfo)
    {
        Console.WriteLine("Start game engine");
    }

    public List<Action> OnFrame(ResponseObservation observation)
    {
        Console.WriteLine($"Frame {observation.Observation.GameLoop}");

        var actions = new List<Action>();

        foreach (var unit in observation.Observation.RawData.Units)
        {
            if (unit.Alliance != Alliance.Self)
                continue;

            if (unit.UnitType != (uint) UnitTypes.PROTOSS_NEXUS)
                continue;

            var command = new ActionRawUnitCommand();
            command.UnitTags.Add(unit.Tag);
            command.AbilityId = (int) Abilities.TRAIN_PROBE;

            var action = new Action {ActionRaw = new() {UnitCommand = command}};

            actions.Add(action);
        }

        return actions;
    }
}