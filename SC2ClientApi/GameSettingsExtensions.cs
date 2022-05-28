using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;
using SC2APIProtocol;

namespace SC2ClientApi;

public static class GameSettingsExtensions
{
    public static string ToArguments(this GameSettings gs, bool isHost)
    {
        var sb = new StringBuilder();
        sb.Append($"{ClientConstants.Address} {gs.ConnectionAddress} ");

        if (isHost)
            sb.Append($"{ClientConstants.Port} {gs.ConnectionServerPort} ");
        else
            sb.Append($"{ClientConstants.Port} {gs.ConnectionClientPort} ");

        if (gs.Fullscreen)
            sb.Append($"{ClientConstants.Fullscreen} 1 ");
        else
            sb.Append($"{ClientConstants.Fullscreen} 0 {ClientConstants.WindowHeight} {gs.ClientWindowHeight} {ClientConstants.WindowVertical} {gs.ClientWindowWidth} ");

        if (!isHost)
            sb.Append($"{ClientConstants.WindowVertical} {gs.ClientWindowWidth} ");
        return sb.ToString();
    }

    public static bool IsMultiplayer(this GameSettings gs) => gs.Opponents.Any(c => c.Type == PlayerType.Participant);

    public static PortSet ServerPort(this GameSettings gs) => new() {GamePort = gs.MultiplayerSharedPort + 1, BasePort = gs.MultiplayerSharedPort + 2};


    public static Collection<PortSet> ClientPorts(this GameSettings gs) => new() {new() {GamePort = gs.ConnectionClientPort + 1, BasePort = gs.ConnectionServerPort + 1}};

    public static Request JoinGameRequest(this GameSettings gs, bool isHost)
    {

        if (gs.IsMultiplayer())
        {
            return new()
            {
                JoinGame = new()
                {
                    Race = isHost ? gs.ParticipantRace : gs.Opponents.First(o => o.Type == PlayerType.Participant).Race,
                    Options = gs.InterfaceOptions,
                    SharedPort = gs.MultiplayerSharedPort,
                    ServerPorts = gs.ServerPort(),
                    ClientPorts = {gs.ClientPorts()}
                }
            };
        }

        return new()
        {
            JoinGame = new()
            {
                Race = gs.ParticipantRace,
                PlayerName = gs.ParticipantName,
                Options = gs.InterfaceOptions
            }
        };
    }

    public static Request CreateGameRequest(this GameSettings gs)
    {
        var r = new Request
        {
            CreateGame = new()
            {
                DisableFog = gs.DisableFog,
                Realtime = gs.Realtime,
                PlayerSetup = {gs.Opponents, new PlayerSetup {Type = PlayerType.Participant, Race = gs.ParticipantRace}}
            }
        };
        if (File.Exists(gs.GameMap) || File.Exists($"{gs.FolderPath}\\Maps\\{gs.GameMap}"))
            r.CreateGame.LocalMap = new() {MapPath = gs.GameMap};
        else
            r.CreateGame.BattlenetMapName = gs.GameMap;
        return r;
    }

    public static Uri GetUri(this GameSettings gs, bool isHost) => isHost
        ? new($"ws://{gs.ConnectionAddress}:{gs.ConnectionServerPort}/sc2api")
        : new Uri($"ws://{gs.ConnectionAddress}:{gs.ConnectionClientPort}/sc2api");

    public static string WorkingDirectory(this GameSettings gs) => $"{gs.FolderPath}\\Support";

    public static string ExecutableClientPath(this GameSettings gs)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return Directory.GetDirectories(gs.FolderPath + @"\Versions\", @"Base*")[0] + @"\SC2.app";

        return Directory.GetDirectories(gs.FolderPath + @"\Versions\", @"Base*")[0] + @"\SC2.exe";
    }
}