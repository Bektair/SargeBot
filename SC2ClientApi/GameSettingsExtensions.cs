using SC2APIProtocol;

namespace SC2ClientApi;

public static class GameSettingsExtensions
{
    public static int GetPort(this GameSettings gs, bool isHost) => isHost ? gs.HostPort : gs.GuestPort;

    public static bool IsMultiplayer(this GameSettings gs) => gs.PlayerTwo is {Type: PlayerType.Participant};
}