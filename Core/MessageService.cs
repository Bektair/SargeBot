using SC2APIProtocol;
using SC2ClientApi;
using Action = SC2APIProtocol.Action;

namespace Core;

internal class MessageService : IMessageService
{
    private GameConnection Connection { get; set; } = new();
    private List<Action> Actions { get; } = new();
    private List<DebugCommand> Debugs { get; } = new();

    public void SetConnection(GameConnection connection)
    {
        Connection = connection;
    }

    public async Task OnFrame(ResponseObservation obs)
    {
        var actions = await Connection.ActionRequest(Actions);

#if DEBUG
        Func<ActionResult, bool> pred = x =>
            x != ActionResult.Success &&
            x != ActionResult.Error && // not sure
            x != ActionResult.NotEnoughMinerals && // macro.build() doesn't check minerals
            x != ActionResult.CantFindPlacementLocation && // building placement spam
            x != ActionResult.CantBuildLocationInvalid && // building placement spam 
            x != ActionResult.NotSupported; // not sure

        if (actions.Any(pred))
            // getting some Error results, but that may be due to action spam when we can't afford units ie.
            Log.Warning($"Unhandled actions: {string.Join(", ", actions.Where(pred))}");

        // Debug panel
        Debugs.Add(DebugRequest.DrawText($"Frame {obs.Observation.GameLoop}"));
        Debugs.Add(DebugRequest.DrawText($"\nActions per frame {Actions.Count} ({actions.Count(x => x != ActionResult.Success)} errors)"));

        await Connection.DebugRequest(Debugs);
        Debugs.Clear();
#endif

        Actions.Clear();
    }

    public void Debug(DebugCommand command)
    {
        Debugs.Add(command);
    }

    public void Action(Ability ability, IEnumerable<ulong> unitTags, Point2D target, bool queue = false)
    {
        var command = CreateActionCommand(ability, unitTags, queue);
        command.TargetWorldSpacePos = target;

        AddActionRaw(command);
    }

    public void Action(Ability ability, IEnumerable<ulong> unitTags, ulong target, bool queue = false)
    {
        var command = CreateActionCommand(ability, unitTags, queue);
        command.TargetUnitTag = target;

        AddActionRaw(command);
    }


    public void Action(Ability ability, IEnumerable<ulong> unitTags, bool queue = false)
    {
        var command = CreateActionCommand(ability, unitTags, queue);

        AddActionRaw(command);
    }

    public void Chat(string message)
    {
        Actions.Add(new Action { ActionChat = new ActionChat { Message = message } });
    }


    private ActionRawUnitCommand CreateActionCommand(Ability ability, IEnumerable<ulong> unitTags, bool queue = false) =>
        new ActionRawUnitCommand
        {
            AbilityId = (int)ability,
            QueueCommand = queue,
            UnitTags = { unitTags }
        };

    private void AddActionRaw(ActionRawUnitCommand command)
    {
        Actions.Add(new Action { ActionRaw = new ActionRaw { UnitCommand = command } });
    }
}

public interface IMessageService
{
    public void Action(Ability ability, IEnumerable<ulong> unitTags, bool queue = false);
    public void Action(Ability ability, IEnumerable<ulong> unitTags, Point2D target, bool queue = false);
    public void Action(Ability ability, IEnumerable<ulong> unitTags, ulong target, bool queue = false);
    void Chat(string message);
    void Debug(DebugCommand command);
    public void SetConnection(GameConnection connection);
    public Task OnFrame(ResponseObservation responseObservation);
}