using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SargeBot.Options;
using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SargeBot.Features.GameInfo;
/// <summary>
/// Populates the static gameinfo data about the map
/// And provides user friendly methods to access the data
/// </summary>
public class MapDataService
{
    public MapData MapData { get; set; }
    private string _dataFolderName;
    private JsonSerializerOptions serializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(),
            new Point2D_DictionarySerializer<MapCell>()
        }
    };

    public MapDataService(MapData Data, IOptions<CacheOptions> cacheSettings)
    {
        this.MapData = Data;
        _dataFolderName = cacheSettings.Value.DataFolderName;
    }

    public void LoadDataFromFile(string mapName)
    {
        Console.WriteLine("load mapdata from file " + mapName + ".json");
        string desJsonString = File.ReadAllText(Path.Combine(_dataFolderName, mapName + ".json"));
        MapData = JsonSerializer.Deserialize<MapData>(desJsonString, serializerOptions);
    }

    public void CreateLoadFile(ResponseGameInfo GameInfo, string mapName)
    {
        MapData data = PopulateMapData(GameInfo);
        string jsonString = JsonSerializer.Serialize(data, serializerOptions);
        File.WriteAllText(Path.Combine(_dataFolderName, mapName + ".json"), jsonString);
    }
    public MapData PopulateMapData(ResponseGameInfo GameInfo)
    {
        ImageData PlacementGrid = GameInfo.StartRaw.PlacementGrid;
        ImageData PathingGrid = GameInfo.StartRaw.PathingGrid;
        ImageData HeightGrid = GameInfo.StartRaw.TerrainHeight;
        MapData.MapName = GameInfo.MapName;
        MapData.MapWidth = PathingGrid.Size.X <= PathingGrid.Size.Y ? PathingGrid.Size.X : PathingGrid.Size.Y;
        MapData.MapLength = PathingGrid.Size.X <= PathingGrid.Size.Y ? PathingGrid.Size.Y : PathingGrid.Size.X;
        var Map = new Dictionary<Point2D, MapCell>();
        MapData.Map = Map;
        for (var x = 0; x < PathingGrid.Size.X; x++)
        {
            for(var y = 0; y < PathingGrid.Size.Y; y++)
            {
                var walkable = GetDataValueBit(PathingGrid, x, y);
                var height = GetDataValueByte(HeightGrid, x, y);
                var placeable = GetDataValueBit(PlacementGrid, x, y);
                Map.Add(new Point2D { X = x, Y = y }, 
                    new MapCell { Height = height, Buildable = placeable, Walkable = walkable });
            }
        }
        return MapData;
    }

    static bool GetDataValueBit(ImageData data, int x, int y)
    {
        int pixelID = x + y * data.Size.X;
        int byteLocation = pixelID / 8;
        int bitLocation = pixelID % 8;
        return ((data.Data[byteLocation] & 1 << (7 - bitLocation)) == 0) ? false : true;
    }
    static int GetDataValueByte(ImageData data, int x, int y)
    {
        int pixelID = x + y * data.Size.X;
        return data.Data[pixelID];
    }

    public int GetHeightZ(Point2D point) => MapData.Map.GetValueOrDefault(point).ZHegith;
    public int GetHeightZ(int X, int Y) => MapData.Map.GetValueOrDefault(new Point2D { X = X, Y = Y }).ZHegith;
}

