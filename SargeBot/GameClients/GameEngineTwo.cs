using SC2APIProtocol;
using SC2ClientApi;
using SC2ClientApi.Constants;
using Action = SC2APIProtocol.Action;

namespace SargeBot.GameClients;

public class GameEngineTwo : IGameEngine
{
    private string asd = "";

    public void OnStart(ResponseGameInfo gameInfo)
    {
        Console.WriteLine("Start game engine");
        asd = Guid.NewGuid().ToString();
    }

    public Request OnFrame(ResponseObservation observation)
    {
        Console.WriteLine($"Frame {observation.Observation.GameLoop}");

        var request = new Request
        {
            // Action = new()
        };

        uint size = 16;
        var x = 0.1f;
        var y = 0.1f;
        var text = asd;

        request.Debug = new()
        {
            Debug =
            {
                new DebugCommand
                {
                    Draw = new() {Text = {new DebugText {Text = text, Size = size, VirtualPos = new() {X = x, Y = y}}}}
                }
            }
        };


        foreach (var unit in observation.Observation.RawData.Units)
        {
            if (unit.Alliance != Alliance.Self)
                continue;

            if (unit.UnitType != (uint) UnitTypes.PROTOSS_NEXUS)
                continue;

            var command = new ActionRawUnitCommand();
            command.UnitTags.Add(unit.Tag);
            command.AbilityId = (int) Abilities.TRAIN_PROBE;

            var action = new Action
            {
                ActionRaw = new()
                {
                    UnitCommand = command
                }
            };

            //request.Action.Actions.Add(action);
        }

        return request;
    }
}