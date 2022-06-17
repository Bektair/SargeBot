using SC2APIProtocol;

namespace SC2ClientApi;

public class GameSettings
{
    // server settings
    public string HostAddress { get; set; }
    public int HostPort { get; set; }
    public int GuestPort { get; set; }
    public InterfaceOptions InterfaceOptions { get; set; }

    // client settings
    public bool Fullscreen { get; set; }
    public int WindowWidth { get; set; }
    public int WindowHeight { get; set; }
    public int WindowX { get; set; }
    public int WindowY { get; set; }

    // game settings
    public string MapName { get; set; }
    public bool DisableFog { get; set; }
    public bool Realtime { get; set; }
    public PlayerSetup PlayerOne { get; set; }
    public PlayerSetup PlayerTwo { get; set; }
}