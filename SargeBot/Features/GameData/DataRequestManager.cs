using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
    private GameDataService _gameData;
    private string _dataFolderName;
    private string _dataFileName;
    private string _dataFilePath;


    private JsonSerializerOptions serializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public DataRequestManager(GameDataService gameData, IServiceProvider services)
    {
        _gameData = gameData;
        var cacheSettings = services.GetRequiredService<IOptions<CacheOptions>>();
        _dataFolderName = cacheSettings.Value.DataFolderName;
        _dataFileName = cacheSettings.Value.DataFileName;
        _dataFilePath = Path.Combine(_dataFolderName, _dataFileName);
    }

    public bool CreateLoadData(ResponseData data)
    {
        CreateDirectoryIfNeeded();
        WriteValuesToFile(data);
        LoadDataFromFile();
        return (_gameData.abilitiesDict.Count > 0 && _gameData.upgradesDict.Count > 0 && _gameData.unitsDict.Count > 0) ;
    }

    public void LoadDataFromResponse(ResponseData dataResponse)
    {
        _gameData.FillAllData(dataResponse);
    }

    public void LoadDataFromFile()
    {
        string desJsonString = File.ReadAllText(_dataFilePath);
        var newData = JsonSerializer.Deserialize<GameDataService>(desJsonString, serializerOptions);
        _gameData.unitsDict = newData.unitsDict;
        _gameData.upgradesDict = newData.upgradesDict;
        _gameData.abilitiesDict = newData.abilitiesDict;
    }

    public void LoadDataFullPath()
    {
        string desJsonString = File.ReadAllText(_dataFilePath);
        var newData = JsonSerializer.Deserialize<GameDataService>(desJsonString, serializerOptions);
        _gameData.unitsDict = newData.unitsDict;
        _gameData.upgradesDict = newData.upgradesDict;
        _gameData.abilitiesDict = newData.abilitiesDict;
    }

    private void WriteValuesToFile(ResponseData dataResponse)
    {
        _gameData.FillAllData(dataResponse);
        string jsonString = JsonSerializer.Serialize(_gameData, serializerOptions);
        File.WriteAllText(_dataFilePath, jsonString);
    }

    private void CreateDirectoryIfNeeded()
    {
        if (!Directory.Exists(_dataFolderName)) { Directory.CreateDirectory(_dataFolderName); }
    }
}
