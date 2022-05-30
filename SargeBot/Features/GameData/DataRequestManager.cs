using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SargeBot.Options;
using SC2ClientApi;

namespace SargeBot.Features.GameData;
public class DataRequestManager
{
    GameClient GameClient;

    public DataRequestManager(GameClient gameClient, IServiceProvider services)
    {
        GameClient = gameClient;
    }

    public static void LoadData ()
    {
        string DataPath = @"data";
        string DataFile = "data.json";
        Console.WriteLine(Directory.GetCurrentDirectory);
        Console.WriteLine(Path.Combine(DataPath, DataFile));
        Console.WriteLine(Path.GetFullPath(Path.Combine(DataPath, DataFile)));
        if (File.Exists(Path.Combine(DataPath, DataFile))){
            Console.WriteLine("You have data file");
        }
        else
        {
            Console.WriteLine("Did not find data file");
        }

    }



    
}
