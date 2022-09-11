using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Intel;
public class MapCell
{
    public int Height { get; set; }
    public bool Buildable { get; set; }
    public bool Walkable { get; set; }
    [JsonIgnore]
    public int ZHegith { get { return (int)Math.Round(Terrain_to_z_height(Height));} }

    //TweakImpsAlg
    decimal Terrain_to_z_height(int terrainHeight)
    {
        return -16M + 32M * terrainHeight / 255M;
    }

}

