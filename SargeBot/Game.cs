using SC2APIProtocol;
using SC2ClientApi;

namespace SargeBot;

/// <summary>
///     Representation of a single-client (AI vs. Blizzard AI) or multi-client (AI vs. Human or AI vs. AI) match.
/// </summary>
public class Game
{
    /// <summary>
    ///     Representation of a StarCraft II match.
    /// </summary>
    /// <param name="playerOne">Host of the game</param>
    /// <param name="playerTwo">Optional opponent</param>
    public Game(GameClient playerOne, GameClient? playerTwo = null)
    {
        PlayerOne = playerOne;
        PlayerTwo = playerTwo;
    }

    /// <summary>
    ///     The first player - always assumed to be host.
    /// </summary>
    private GameClient PlayerOne { get; }

    /// <summary>
    ///     The second player - always assumed to be 'client'.
    /// </summary>
    private GameClient? PlayerTwo { get; }

    /// <summary>
    ///     Used to start the match.
    ///     IClients are expected to handle everything from here.
    /// </summary>
    public async Task ExecuteMatch()
    {
        var players = new List<GameClient> {PlayerOne};
        if (PlayerTwo != null)
            players.Add(PlayerTwo);

        // Launch StarCraft II clients and connect
        await Task.WhenAll(players.Select(p => p.Initialize()));

        // Host creates the game
        var host = players.First(p => p.IsHost);

        //Can get the mapName from here to cache the static mapInfo
        await host.CreateGameRequest();

        // Join game
        await Task.WhenAll(players.Select(p => p.JoinGameRequest()));

        // Run game loop
        await Task.WhenAll(players.Select(p => p.Run()));
    }
}