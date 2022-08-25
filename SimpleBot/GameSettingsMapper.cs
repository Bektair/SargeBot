using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SC2ClientApi;
using SimpleBot.Options;

namespace SimpleBot;

public static class GameSettingsMapper
{
    public static GameSettings CreateGameSettings(this IServiceProvider services)
    {
        var serverOptions = services.GetRequiredService<IOptions<ServerOptions>>().Value;
        var sc2ProcessOptions = services.GetRequiredService<IOptions<Sc2ProcessOptions>>().Value;
        var gameOptions = services.GetRequiredService<IOptions<GameOptions>>().Value;

        return new()
        {
            HostAddress = serverOptions.ServerAddress ?? "127.0.0.1",
            HostPort = serverOptions.GamePort,
            GuestPort = serverOptions.StartPort,

            Fullscreen = sc2ProcessOptions.Fullscreen,
            WindowWidth = sc2ProcessOptions.WindowWidth,
            WindowHeight = sc2ProcessOptions.WindowHeight,
            WindowX = sc2ProcessOptions.WindowX,
            WindowY = sc2ProcessOptions.WindowY,

            InterfaceOptions = gameOptions.Join,
            MapName = gameOptions.Create.MapName,
            Realtime = gameOptions.Create.Realtime,
            DisableFog = gameOptions.Create.DisableFog,
            PlayerOne = gameOptions.PlayerOne,
            PlayerTwo = gameOptions.PlayerTwo
        };
    }
}