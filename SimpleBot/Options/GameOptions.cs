using SC2APIProtocol;

namespace SimpleBot.Options;

public class GameOptions
{
    public PlayerSetup PlayerOne { get; set; }
    public PlayerSetup PlayerTwo { get; set; }
    public CreateOptions Create { get; set; }
    public InterfaceOptions Join { get; set; }

    public class CreateOptions
    {
        public string MapName { get; set; }
        public bool Realtime { get; set; }
        public bool DisableFog { get; set; }
    }
}