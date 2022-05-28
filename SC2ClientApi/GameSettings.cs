using SC2APIProtocol;

namespace SC2ClientApi;

public class GameSettings
{
    // Technical Settings
    public string FolderPath { get; set; }
    public string ConnectionAddress { get; set; }
    public int ConnectionServerPort { get; set; }
    public int ConnectionClientPort { get; set; }
    public int MultiplayerSharedPort { get; set; }
    public InterfaceOptions InterfaceOptions { get; set; }

    // Client Settings
    public bool Fullscreen { get; set; }
    public int ClientWindowWidth { get; set; }
    public int ClientWindowHeight { get; set; }

    // Game Settings
    public string GameMap { get; set; }
    public bool DisableFog { get; set; }
    public bool Realtime { get; set; }
    public Race ParticipantRace { get; set; }
    public string ParticipantName { get; set; }
    public List<PlayerSetup> Opponents { get; set; } = new();
}