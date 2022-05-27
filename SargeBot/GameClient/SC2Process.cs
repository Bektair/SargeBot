using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SargeBot.Options;
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
/// Contains a game engine abstraction to run the gameloop
/// 
/// </summary>

public class SC2Process
{

    private Process? process;
    public GameEngine gameEngine { get; }

    private string starcraftExe = "";
    private string starcraftDir = "";

    public string mapPath { get; }

    public SC2Process(IOptions<GameConnectionOptions> ConnOptions, IOptions<RequestOptions> ReqOptions)
    {
        Start(ConnOptions.Value.Address, ConnOptions.Value.Port);
        mapPath = CreateMapPath(ReqOptions.Value.Create.MapName);
    }

    public void Start(string address, int port)
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
        processStartInfo.Arguments = String.Format("-listen {0} -port {1} -displayMode 0", address, port);
        processStartInfo.WorkingDirectory = Path.Combine(starcraftDir, "Support64");
        process = Process.Start(processStartInfo);
    }
    //GetMapPath() is temporally coupled with Start()
    private string CreateMapPath(String mapName)
    {
        string mapPath = Path.Combine(starcraftDir, "Maps", mapName);
        if (!File.Exists(mapPath))
        {
            throw new Exception("Could not find map at " + mapPath);
        }
        return mapPath;
    }


    public Process? GetProcess()
    {
        return process;
    }
}

