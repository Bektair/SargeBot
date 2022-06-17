using System.Text;
using SC2APIProtocol;
using SC2ClientApi.Constants;

namespace SC2ClientApi;

public static class GameSettingsExtensions
{
    public static int GetPort(this GameSettings gs, bool isHost) => isHost ? gs.GamePort : gs.StartPort;

    public static string ToArguments(this GameSettings gs, bool isHost)
    {
        var sb = new StringBuilder();
        sb.Append($"{ClientConstants.Address} {gs.ServerAddress} ");

        if (isHost)
            sb.Append($"{ClientConstants.Port} {gs.GamePort} ");
        else
            sb.Append($"{ClientConstants.Port} {gs.StartPort} ");

        if (gs.Fullscreen)
            sb.Append($"{ClientConstants.Fullscreen} 1 ");
        else
            sb.Append($"{ClientConstants.Fullscreen} 0 {ClientConstants.WindowWidth} {gs.WindowWidth} ");

        if (!isHost)
            sb.Append($"{ClientConstants.WindowX} {gs.WindowWidth} ");
        return sb.ToString();
    }

    public static bool IsMultiplayer(this GameSettings gs) => gs.PlayerTwo is {Type: PlayerType.Participant};
}