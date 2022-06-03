using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SargeBot.Options;
using SC2APIProtocol;
using SC2ClientApi;
using SC2ClientApi.Constants;

namespace SargeBot.Features.GameData;
/// <summary>
/// Sends a dataRequest, caches the data for each version in file
/// </summary>
public class DataRequestManager
{
    private GameData _gameData;
    private string gameVersion;
    private string dataVersion;

    public DataRequestManager(GameData gameData)
    {
        _gameData = gameData;
    }

    public async Task LoadData ()
    {
        dataVersion = await GetDataVersion();
        string DataFileName = "data_"+ dataVersion + ".json";
        if (File.Exists(Path.Combine(@"data", DataFileName)))
        {
            Console.WriteLine("You have data file allready");
            //Load
        }else {
            // var filePath = CreateFileAndDirectory(DataFileName);
            // //GetData
            // var dataResponse = await gameClient.SendAndReceive(ClientConstants.RequestData);
            // //Write data
            // writeValuesToFile(filePath, dataResponse);
            // //Load
        }
    }

    private void writeValuesToFile(string filePath, Response dataResponse)
    {
        RepeatedField<AbilityData> abilities = dataResponse.Data.Abilities;
        _gameData.FillAbilities(abilities);
        string jsonString = JsonSerializer.Serialize(_gameData);
        Console.WriteLine(jsonString);
        //FileStream write = File.OpenWrite(filePath);
        RepeatedField<UnitTypeData> units = dataResponse.Data.Units;
        _gameData.FillUnits(units);
        RepeatedField<UpgradeData> upgrades = dataResponse.Data.Upgrades;
        _gameData.FillUpgrades(upgrades);

    }

    public string CreateFileAndDirectory(string DataFileName)
    {
        Console.WriteLine("cwd:" + Directory.GetCurrentDirectory());
        string DataFolderPath = CreateDirectory();
        Console.WriteLine("FolderPathRelative:" + Path.Combine(DataFolderPath, DataFileName));
        string fullPath = Path.GetFullPath(Path.Combine(DataFolderPath, DataFileName));
        Console.WriteLine("FolderPathAbs:" + fullPath);
        return CreateFile(fullPath);
    }

    private string CreateDirectory()
    {
        string DataFolderPath = @"data";
        if (!Directory.Exists(DataFolderPath)) { Directory.CreateDirectory(DataFolderPath); }
        return DataFolderPath;
    }

    private string CreateFile(string DateFilePath)
    {
        Console.WriteLine("Did not find data file, creating one");
        File.Create(DateFilePath);
        return DateFilePath;
    }

    protected async Task<string> GetDataVersion()
    {
        // if(gameClient.PingResponse!=null) return gameClient.PingResponse.Ping.DataVersion;
        // var response = await gameClient.SendAndReceive(ClientConstants.RequestPing);
        // if (response != null ) 
        //     return response.Ping.DataVersion;
        // else 
        //     Console.WriteLine("Ping failure, loadData not possible");
        return "";
    }


    
}
