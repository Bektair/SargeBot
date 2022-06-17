using System.Diagnostics;
using System.Runtime.InteropServices;
using SC2ClientApi.Constants;

namespace SC2ClientApi;

public static class Sc2Process
{
    private static string? _sc2Directory;

    public static void Start(GameSettings gs, bool isHost)
    {
        var sc2Exe = Sc2Exe();
        var workingDirectory = WorkingDirectory();
        var arguments = Arguments(gs, isHost);

        Log.Info($"Starting sc2 process {arguments}");
        Process.Start(new ProcessStartInfo(sc2Exe)
        {
            Arguments = arguments, WorkingDirectory = workingDirectory
        });
    }

    public static string MapDirectory() => @$"{Sc2Directory()}\Maps";

    private static string Arguments(GameSettings gs, bool isHost)
    {
        var arguments = new List<string>
        {
            ClientConstants.Address,
            gs.ServerAddress,
            ClientConstants.Port,
            (isHost ? gs.GamePort : gs.StartPort).ToString(),
            ClientConstants.Fullscreen,
            gs.Fullscreen ? "1" : isHost ? "0" : $"0 {ClientConstants.WindowX} {gs.WindowWidth + gs.WindowX}"
        };

        return string.Join(" ", arguments);
    }


    private static string WorkingDirectory() => @$"{Sc2Directory()}\Support64";

    private static string Sc2Exe()
    {
        var versionDirectory = Directory.GetDirectories(@$"{Sc2Directory()}\Versions\", @"Base*")[0];
        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            ? @$"{versionDirectory}\SC2.app"
            : @$"{versionDirectory}\SC2_x64.exe";
    }

    private static string? Sc2Directory()
    {
        if (_sc2Directory != null) return _sc2Directory;

        var myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var executeInfo = Path.Combine(myDocuments, "Starcraft II", "ExecuteInfo.txt");
        if (File.Exists(executeInfo))
        {
            var lines = File.ReadAllLines(executeInfo);
            foreach (var line in lines)
            {
                var argument = line[(line.IndexOf('=') + 1)..].Trim();
                if (line.Trim().StartsWith("executable"))
                {
                    _sc2Directory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(argument)));
                    if (_sc2Directory != null) return _sc2Directory;
                }
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            _sc2Directory = Path.Combine(programFiles, "StarCraft II");
            return _sc2Directory;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var applications = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            _sc2Directory = Path.Combine(applications, "StarCraft II");
            return _sc2Directory;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // implement linux
            return null;
        }

        throw new("Not using a supported OS");
    }
}