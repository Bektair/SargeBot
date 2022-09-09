namespace Core;

public abstract class BaseBuildState
{
    protected BaseBuildState(BaseBot bot)
    {
        Bot = bot;
    }

    protected BaseBot Bot { get; }

    public abstract void OnFrame();

    public abstract bool NextState();
}