using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Options;
public class RequestOptions
{
    public const string RequestSettings = "RequestSettings";

    public PlayerSetup Host { get; set; }
    public PlayerSetup Client { get; set; }
    public PlayerSetup AIClient { get; set; }

    public CreateSettings Create { get; set; }

    public InterfaceOptions Join { get; set; }

    public class CreateSettings {

        public string MapName { get; set; }    

        public bool Realtime { get; set; } = false;

    }



}

