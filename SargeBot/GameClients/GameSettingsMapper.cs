using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SargeBot.Options;
using SC2APIProtocol;
using SC2ClientApi;

namespace SargeBot.GameClients;

public static class GameSettingsMapper
{
    public static GameSettings CreateGameSettings(this IServiceProvider services)
    {
        var requestOptions = services.GetRequiredService<IOptions<RequestOptions>>();
        var gameConnectionOptions = services.GetRequiredService<IOptions<GameConnectionOptions>>();
        var processSettings = services.GetRequiredService<IOptions<ProcessOptions>>();

        return new()
        {
            FolderPath = processSettings.Value.FolderPath,
            Fullscreen = processSettings.Value.Fullsceen,
            ClientWindowWidth = processSettings.Value.ClientWindowWith,
            ClientWindowHeight = processSettings.Value.ClientWindowHeight,
            ConnectionAddress = IPAddress.Loopback.ToString(),
            ConnectionServerPort = gameConnectionOptions.Value.ServerPort,
            ConnectionClientPort = gameConnectionOptions.Value.ClientPort,
            MultiplayerSharedPort = gameConnectionOptions.Value.SharedPort,
            InterfaceOptions = requestOptions.Value.Join,
            GameMap = requestOptions.Value.Create.MapName,
            Realtime = requestOptions.Value.Create.Realtime,
            DisableFog = requestOptions.Value.Create.DisableFog,

            PlayerOne = new()
            {
                Type = PlayerType.Participant,
                Race = requestOptions.Value.Host.Race,
                PlayerName = requestOptions.Value.Host.PlayerName
            },
            PlayerTwo = new()
            {
                Type = PlayerType.Participant,
                Race = requestOptions.Value.Client.Race,
                PlayerName = requestOptions.Value.Client.PlayerName
            }
        };
    }
}