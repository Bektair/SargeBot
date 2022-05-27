using Microsoft.Extensions.Options;
using SargeBot.Options;
using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.GameClient;
public class GameEngineFactory
{
    IGameConnection gameConnection;
    IOptions<OpponentPlayerOptions> options;

    public GameEngineFactory(IGameConnection gameConnection, IOptions<OpponentPlayerOptions> options)
    {
        this.gameConnection = gameConnection;
        this.options = options;
    }

    public GameEngine MakeGameEngine(string MapPath)
    {
        return new GameEngine(gameConnection, options, MapPath);
    }
}

