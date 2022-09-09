namespace Core;

public abstract class BaseBuildState
{
    protected BaseBuildState(BaseBot bot, params Func<bool>[] activationTriggers)
    {
        Bot = bot;
        ActivationTriggers = activationTriggers.Any() ? activationTriggers : new[] { DefaultTrigger(bot) };
    }

    protected BaseBot Bot { get; }
    private IEnumerable<Func<bool>> ActivationTriggers { get; }

    public abstract void OnFrame();

    public bool ShouldActivate()
    {
        return ActivationTriggers.Any(predicate => predicate());
    }

    protected virtual Func<bool> DefaultTrigger(BaseBot bot) => () => false;
}