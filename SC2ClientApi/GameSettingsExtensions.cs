using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;
using SC2APIProtocol;
using SC2ClientApi.Constants;

namespace SC2ClientApi;

public static class GameSettingsExtensions
{
    public static int GetPort(this GameSettings gs, bool isHost) => isHost ? gs.ConnectionServerPort : gs.ConnectionClientPort;


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
            sb.Append($"{ClientConstants.Fullscreen} 0 {ClientConstants.WindowWidth} {gs.WindowWidth} ");

        if (!isHost)
            sb.Append($"{ClientConstants.WindowX} {gs.WindowWidth} ");
        return sb.ToString();
    }

    public static bool IsMultiplayer(this GameSettings gs) => gs.PlayerTwo is {Type: PlayerType.Participant};

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
                    Race = isHost ? gs.PlayerOne.Race : gs.PlayerTwo.Race,
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
                Race = gs.PlayerOne.Race,
                PlayerName = gs.PlayerOne.PlayerName,
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
                PlayerSetup = {gs.PlayerOne, gs.PlayerTwo}
            }
        };

        var folderPath = @"C:\Program Files (x86)\StarCraft II";
        if (File.Exists(gs.MapName) || File.Exists($"{folderPath}\\Maps\\{gs.MapName}"))
            r.CreateGame.LocalMap = new() {MapPath = gs.MapName};
        else
            r.CreateGame.BattlenetMapName = gs.MapName;
        return r;
    }

    public static Uri GetUri(this GameSettings gs, bool isHost) => isHost
        ? new($"ws://{gs.ConnectionAddress}:{gs.ConnectionServerPort}/sc2api")
        : new Uri($"ws://{gs.ConnectionAddress}:{gs.ConnectionClientPort}/sc2api");

    public static string WorkingDirectory(this GameSettings gs) => $"{Sc2Directory()}\\Support";

    public static string ExecutableClientPath(this GameSettings gs)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return Directory.GetDirectories(Sc2Directory() + @"\Versions\", @"Base*")[0] + @"\SC2.app";

        return Directory.GetDirectories(Sc2Directory() + @"\Versions\", @"Base*")[0] + @"\SC2.exe";
    }

    public static string Sc2Directory()
    {
        string starcraftExe;
        var myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var executeInfo = Path.Combine(myDocuments, "Starcraft II", "ExecuteInfo.txt");
        if (File.Exists(executeInfo))
        {
            var lines = File.ReadAllLines(executeInfo);
            foreach (var line in lines)
            {
                var argument = line.Substring(line.IndexOf('=') + 1).Trim();
                if (line.Trim().StartsWith("executable"))
                {
                    starcraftExe = argument;
                    var starcraftFolder = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(starcraftExe)));
                    return starcraftFolder;
                }
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            return Path.Combine(programFiles, "StarCraft II");
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            //Mac
            var applications = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            return Path.Combine(applications, "StarCraft II");
        }
        // ladder plays on linux

        throw new("Not using a supported OS");
    }
}