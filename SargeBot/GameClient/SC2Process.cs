using Microsoft.Extensions.Hosting;
using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.GameClient;

/// <summary>
/// 
/// Starting point to open SC2.
/// 
/// </summary>

public class SC2Process
{

    private Process? process;
    private IGameConnection gameConnection;
    public GameEngine gameEngine { get; }

    private string starcraftExe = "";
    private string starcraftDir = "";

    public SC2Process(IGameConnection gameConnection, GameEngine gameEngine)
    {
        this.gameConnection = gameConnection;
        this.gameEngine = gameEngine;
        Start();
    }

    public void Start()
    {
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
                    var nullableStr = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(starcraftExe)));
                    if (nullableStr != null) starcraftDir = nullableStr;
                }
            }
        }


        var processStartInfo = new ProcessStartInfo(starcraftExe);
        processStartInfo.Arguments = String.Format("-listen {0} -port {1} -displayMode 0", gameConnection.getAddress(), gameConnection.getPort());
        processStartInfo.WorkingDirectory = Path.Combine(starcraftDir, "Support64");
        process = Process.Start(processStartInfo);
    }

    public string GetMapPath(String mapName)
    {
        string mapPath = Path.Combine(starcraftDir, "Maps", mapName);
        if (!File.Exists(mapPath))
        {
            throw new Exception("Could not find map at " + mapPath);
        }
        return mapPath;
    }

    public IGameConnection GetGameConnection()
    {
        return gameConnection;
    }

    public Process? GetProcess()
    {
        return process;
    }
}

