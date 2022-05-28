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

        return new()
        {
            FolderPath = @"C:\Program Files (x86)\StarCraft II",
            ConnectionAddress = IPAddress.Loopback.ToString(),
            ConnectionServerPort = gameConnectionOptions.Value.ServerPort,
            ConnectionClientPort = gameConnectionOptions.Value.ClientPort,
            MultiplayerSharedPort = gameConnectionOptions.Value.SharedPort,
            InterfaceOptions = new() {Raw = true, Score = true}, //requestOptions.Value.Join,
            Fullscreen = false,
            ClientWindowWidth = 1024,
            ClientWindowHeight = 768,
            GameMap = requestOptions.Value.Create.MapName,
            Realtime = requestOptions.Value.Create.Realtime,
            DisableFog = false,
            ParticipantRace = requestOptions.Value.Host.Race,
            ParticipantName = requestOptions.Value.Host.PlayerName,
            Opponents = new() {requestOptions.Value.AIClient}
        };
    }
}