using SC2APIProtocol;

namespace SargeBot.Options;

public class RequestOptions
{
    public const string RequestSettings = "RequestSettings";

    public PlayerSetup PlayerOne { get; set; }

    public PlayerSetup PlayerTwo { get; set; }

    public CreateSettings Create { get; set; }

    public InterfaceOptions Join { get; set; }

    public class CreateSettings
    {
        public string MapName { get; set; }
        public bool Realtime { get; set; } = false;
        public bool DisableFog { get; set; }
    }
}