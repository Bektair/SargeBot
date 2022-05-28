using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SargeBot.Options;
using SC2ClientApi;

namespace SargeBot.GameClient;

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
            ParticipantRace = requestOptions.Value.Host.Race,
            ParticipantName = requestOptions.Value.Host.PlayerName,
            Opponents = new() {requestOptions.Value.AIClient}
        };
    }
}