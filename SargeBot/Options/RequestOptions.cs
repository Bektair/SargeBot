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

    public CreateSettings Create { get; set; }

    public bool Realtime { get; set; } = true;


    public RequestOptions() { }

    public class CreateSettings {
        public PlayerSettings Host { get; set; }
        public PlayerSettings Client { get; set; }

        public class PlayerSettings
        {
            public PlayerType PlayerType { get; set; }
        }

    }









}

