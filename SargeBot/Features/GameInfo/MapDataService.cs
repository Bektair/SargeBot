﻿using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SargeBot.Options;
using SC2APIProtocol;

namespace SargeBot.Features.GameInfo;

/// <summary>
///     Populates the static gameinfo data about the map
///     And provides user friendly methods to access the data
/// </summary>
public class MapDataService
{
    private readonly string _dataFolderName;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(),
            new Point2D_DictionarySerializer<MapCell>()
        }
    };

    public MapDataService(IOptions<CacheOptions> cacheSettings)
    {
        _dataFolderName = cacheSettings.Value.DataFolderName;
    }

    public MapData MapData { get; set; } = new();

    public void LoadDataFromFile(string mapName)
    {
        Console.WriteLine("load mapdata from file " + mapName + ".json");
        var desJsonString = File.ReadAllText(Path.Combine(_dataFolderName, mapName + ".json"));
        MapData = JsonSerializer.Deserialize<MapData>(desJsonString, _serializerOptions);
    }

    public void Save()
    {
        if (!Directory.Exists(_dataFolderName)) Directory.CreateDirectory(_dataFolderName);

        var json = JsonSerializer.Serialize(MapData, _serializerOptions);
        File.WriteAllText(Path.Combine(_dataFolderName, $"{MapData.MapName}.json"), json);
    }

    public MapData PopulateMapData(ResponseGameInfo gameInfo)
    {
        var placementGrid = gameInfo.StartRaw.PlacementGrid;
        var pathingGrid = gameInfo.StartRaw.PathingGrid;
        var heightGrid = gameInfo.StartRaw.TerrainHeight;
        MapData.MapName = gameInfo.MapName;
        MapData.MapWidth = pathingGrid.Size.X <= pathingGrid.Size.Y ? pathingGrid.Size.X : pathingGrid.Size.Y;
        MapData.MapLength = pathingGrid.Size.X <= pathingGrid.Size.Y ? pathingGrid.Size.Y : pathingGrid.Size.X;
        var map = new Dictionary<Point2D, MapCell>();
        MapData.Map = map;
        for (var x = 0; x < pathingGrid.Size.X; x++)
        for (var y = 0; y < pathingGrid.Size.Y; y++)
        {
            var walkable = GetDataValueBit(pathingGrid, x, y);
            var height = GetDataValueByte(heightGrid, x, y);
            var placeable = GetDataValueBit(placementGrid, x, y);
            map.Add(new() {X = x, Y = y},
                new() {Height = height, Buildable = placeable, Walkable = walkable});
        }

        return MapData;
    }

    private static bool GetDataValueBit(ImageData data, int x, int y)
    {
        var pixelId = x + y * data.Size.X;
        var byteLocation = pixelId / 8;
        var bitLocation = pixelId % 8;
        return (data.Data[byteLocation] & (1 << (7 - bitLocation))) != 0;
    }

    private static int GetDataValueByte(ImageData data, int x, int y)
    {
        var pixelId = x + y * data.Size.X;
        return data.Data[pixelId];
    }

    public int GetHeightZ(Point2D point) => MapData.Map.GetValueOrDefault(point).ZHegith;
    public int GetHeightZ(int x, int y) => MapData.Map.GetValueOrDefault(new() {X = x, Y = y}).ZHegith;
}