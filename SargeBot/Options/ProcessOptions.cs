using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Options;

public class ProcessOptions
{
    public const string ProcessSettings = "ProcessSettings";
    public string FolderPath { get; }
    public bool Fullsceen { get; set;  } 
    public int ClientWindowWith { get; set; }
    public int ClientWindowHeight { get; set; }

    public ProcessOptions()
    {
        FolderPath = ReadSettings();
    }

    private string ReadSettings()
    {
        string starcraftExe;
        var myDocuments = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        var executeInfo = Path.Combine(myDocuments, "Starcraft II", "ExecuteInfo.txt");
        if (File.Exists(executeInfo))
        {
            var lines = File.ReadAllLines(executeInfo);
            foreach (string line in lines)
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
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            var programFiles = Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86);
            return Path.Combine(programFiles, "StarCraft II");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)){//Mac
            var applications = Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles);
            return Path.Combine(applications, "StarCraft II");
        }
        else
        {
            throw new Exception("Not using a supported OS");
        }
        
    }
}

