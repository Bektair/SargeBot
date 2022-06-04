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
    private string gameVersion;
    private string dataVersion;
    private JsonSerializerOptions serializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    public DataRequestManager(GameDataService gameData)
    {
        _gameData = gameData;
    }

    public void CreateLoadData(ResponseData data, string dataFileName)
    {
        var dir = CreateDirectoryIfNeeded();
        writeValuesToFile(Path.Combine(dir, dataFileName), data);
        LoadDataFromFile(dataFileName);
    }

    public void LoadDataFromResponse(ResponseData dataResponse)
    {
        RepeatedField<UpgradeData> upgrades = dataResponse.Upgrades;
        _gameData.FillUpgrades(upgrades);
        RepeatedField<AbilityData> abilities = dataResponse.Abilities;
        _gameData.FillAbilities(abilities);
        RepeatedField<UnitTypeData> units = dataResponse.Units;
        _gameData.FillUnits(units);
    }

    public void LoadDataFromFile(string dataFileName)
    {
        var dir = CreateDirectoryIfNeeded();
        string desJsonString = File.ReadAllText(Path.Combine(dir, dataFileName));
        var newData = JsonSerializer.Deserialize<GameDataService>(desJsonString, serializerOptions);
        _gameData.unitsDict = newData.unitsDict;
        _gameData.upgradesDict = newData.upgradesDict;
        _gameData.abilitiesDict = newData.abilitiesDict;
    }

    public void LoadDataFullPath(string fullPath)
    {
        string desJsonString = File.ReadAllText(fullPath);
        var newData = JsonSerializer.Deserialize<GameDataService>(desJsonString, serializerOptions);
        _gameData.unitsDict = newData.unitsDict;
        _gameData.upgradesDict = newData.upgradesDict;
        _gameData.abilitiesDict = newData.abilitiesDict;
    }

    private void writeValuesToFile(string filePath, ResponseData dataResponse)
    {
        RepeatedField<UpgradeData> upgrades = dataResponse.Upgrades;
        _gameData.FillUpgrades(upgrades);
        RepeatedField<AbilityData> abilities = dataResponse.Abilities;
        _gameData.FillAbilities(abilities);
        RepeatedField<UnitTypeData> units = dataResponse.Units;
        _gameData.FillUnits(units);
        string jsonString = JsonSerializer.Serialize(_gameData, serializerOptions);
        File.WriteAllText(filePath, jsonString);
    }

    private string CreateDirectoryIfNeeded()
    {
        string DataFolderPath = @"data";
        if (!Directory.Exists(DataFolderPath)) { Directory.CreateDirectory(DataFolderPath); }
        return DataFolderPath;
    }
}
