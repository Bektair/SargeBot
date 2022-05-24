using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Options;
public class GameConnectionOptions
{
    public GameConnectionOptions() { }

    public const string GameConnection = "GameConnection";

    public string address { get; set; } = String.Empty;
    public int port { get; set; } = 5678;
    



}

