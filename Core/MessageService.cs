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

    public async Task OnFrame()
    {
        await Connection.ActionRequest(Actions);
        await Connection.DebugRequest(Debugs);
        Actions.Clear();
        Debugs.Clear();
    }

    public void Debug(DebugCommand command)
    {
        Debugs.Add(command);
    }

    public void Action(Ability ability, IEnumerable<ulong> unitTags, Point2D target, bool queue = false)
    {
        var command = CreateActionCommand(ability, unitTags, queue);
        command.TargetWorldSpacePos = target;

        AddAction(command);
    }

    public void Action(Ability ability, IEnumerable<ulong> unitTags, ulong target, bool queue = false)
    {
        var command = CreateActionCommand(ability, unitTags, queue);
        command.TargetUnitTag = target;

        AddAction(command);
    }


    public void Action(Ability ability, IEnumerable<ulong> unitTags, bool queue = false)
    {
        var command = CreateActionCommand(ability, unitTags, queue);

        AddAction(command);
    }


    private ActionRawUnitCommand CreateActionCommand(Ability ability, IEnumerable<ulong> unitTags, bool queue = false)
    {
        return new ActionRawUnitCommand
        {
            AbilityId = (int)ability,
            QueueCommand = queue,
            UnitTags = { unitTags }
        };
    }

    private void AddAction(ActionRawUnitCommand command)
    {
        Actions.Add(new Action { ActionRaw = new ActionRaw { UnitCommand = command } });
    }
}

public interface IMessageService
{
    public void Action(Ability ability, IEnumerable<ulong> unitTags, bool queue = false);
    public void Action(Ability ability, IEnumerable<ulong> unitTags, Point2D target, bool queue = false);
    public void Action(Ability ability, IEnumerable<ulong> unitTags, ulong target, bool queue = false);
    void Debug(DebugCommand command);
    public void SetConnection(GameConnection connection);
    public Task OnFrame();
}