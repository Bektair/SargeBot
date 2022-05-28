namespace SargeBot.Options;

public class GameConnectionOptions
{
    public const string GameConnection = "GameConnection";

    public int ServerPort { get; set; } = 5678;
    public int ClientPort { get; set; } = 5679;
    public int SharedPort { get; set; } = 5680;
}