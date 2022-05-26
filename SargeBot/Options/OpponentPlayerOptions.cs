using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Options;
public class OpponentPlayerOptions
{
    public OpponentPlayerOptions() { }

    public const string OpponentPlayerSettings = "OpponentPlayerSettings";

    public Race Race { get; set;} = Race.Protoss;

    public AIBuild AIBuild { get; set; } = AIBuild.RandomBuild;

    public Difficulty Difficulty { get; set; } = Difficulty.Easy;

    public PlayerType PlayerType { get; set; } = PlayerType.Computer;

}
