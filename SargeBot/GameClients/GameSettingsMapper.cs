using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SargeBot.Options;
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
            Fullscreen = processSettings.Value.Fullsceen,
            ServerAddress = IPAddress.Loopback.ToString(),
            GamePort = gameConnectionOptions.Value.ServerPort,
            StartPort = gameConnectionOptions.Value.ClientPort,
            InterfaceOptions = requestOptions.Value.Join,
            MapName = requestOptions.Value.Create.MapName,
            Realtime = requestOptions.Value.Create.Realtime,
            DisableFog = requestOptions.Value.Create.DisableFog,
            PlayerOne = requestOptions.Value.PlayerOne,
            PlayerTwo = requestOptions.Value.PlayerTwo
        };
    }
}