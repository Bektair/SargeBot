using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.GameInfo;
/// <summary>
/// Populates the static gameinfo data about the map
/// And provides user friendly methods to access the data
/// </summary>
public class MapService
{
    public MapData MapData;

    public MapService(MapData Data)
    {
        this.MapData = Data;
    }
    public MapData PopulateMapData(Response GameInfoResponse)
    {
        var GameInfo = GameInfoResponse.GameInfo;

      
        ImageData PlacementGrid = GameInfo.StartRaw.PlacementGrid;
        ImageData PathingGrid = GameInfo.StartRaw.PathingGrid;
        ImageData HeightGrid = GameInfo.StartRaw.TerrainHeight;
        MapData.MapName = GameInfo.MapName;
        MapData.MapWidth = PathingGrid.Size.X <= PathingGrid.Size.Y ? PathingGrid.Size.X : PathingGrid.Size.Y;
        MapData.MapLength = PathingGrid.Size.X <= PathingGrid.Size.Y ? PathingGrid.Size.Y : PathingGrid.Size.X;
        var Map = new Dictionary<Point2D, MapCell>();
        for(var x = 0; x < PathingGrid.Size.X; x++)
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
}

