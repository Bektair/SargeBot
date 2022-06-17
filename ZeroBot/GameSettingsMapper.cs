using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SC2ClientApi;
using ZeroBot.Options;

namespace ZeroBot;

public static class GameSettingsMapper
{
    public static GameSettings CreateGameSettings(this IServiceProvider services)
    {
        var serverOptions = services.GetRequiredService<IOptions<ServerOptions>>().Value;
        var sc2ProcessOptions = services.GetRequiredService<IOptions<Sc2ProcessOptions>>().Value;
        var gameOptions = services.GetRequiredService<IOptions<GameOptions>>().Value;

        return new()
        {
            ServerAddress = serverOptions.ServerAddress ?? "127.0.0.1",
            GamePort = serverOptions.GamePort,
            StartPort = serverOptions.StartPort,

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