using SC2APIProtocol;

namespace Core;

public class MicroService : IMicroService
{
    private readonly IMessageService _messageService;

    public MicroService(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public void Move(Squad squad, Point2D target, bool queue = false)
    {
        Order(Ability.move_Move, squad.Units, target, queue);
    }

    public void AttackMove(Squad squad, Point2D target, bool queue = false)
    {
        Order(Ability.ATTACK, squad.Units, target, queue);
    }

    public void Order(Ability ability, IEnumerable<IUnit> units, Point2D target, bool queue = false)
    {
        _messageService.Action(ability, units.Select(x => x.Tag), target, queue);
    }
}

public interface IMicroService
{
    public void Move(Squad squad, Point2D target, bool queue = false);
    public void AttackMove(Squad squad, Point2D target, bool queue = false);
    void Order(Ability ability, IEnumerable<IUnit> units, Point2D target, bool queue = false);
}