using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.GameInfo;

/// <summary>
///  Represents the properties of the ground of the map itself, like if it has creep its height etc
/// </summary>
public class MapData
{
    public int MapWidth { get; set; }
    public int MapLength { get; set; }
    public string MapName { get; set; }

    public Dictionary<Point2D, MapCell> Map { get; set; }    //65,536 entries



    public MapData()
    {
    }



}

