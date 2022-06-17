using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using SC2ClientApi.Constants;

namespace SC2ClientApi;

public static class Sc2Process
{
    private static string? _sc2Directory;

    public static void Start(string arguments)
    {
        var sc2Exe = Sc2Exe();
        var workingDirectory = WorkingDirectory();

        Console.WriteLine($"[{DateTime.Now:T}] Starting sc2 process {arguments}");
        Process.Start(new ProcessStartInfo(sc2Exe)
        {
            Arguments = arguments, WorkingDirectory = workingDirectory
        });
    }

    public static string MapDirectory() => @$"{Sc2Directory()}\Maps";

    //todo: array join on space
    public static string Arguments(GameSettings gs, bool isHost)
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