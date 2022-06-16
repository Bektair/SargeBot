using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SC2ClientApi;
using ZeroBot.Options;

namespace ZeroBot;

public static class GameSettingsMapper
{
    public static GameSettings CreateGameSettings(this IServiceProvider services)
    {
        var gameOptions = services.GetRequiredService<IOptions<GameOptions>>().Value;
        var serverOptions = services.GetRequiredService<IOptions<ServerOptions>>().Value;
        var sc2ProcessOptions = services.GetRequiredService<IOptions<Sc2ProcessOptions>>().Value;

        return new()
        {
            ConnectionAddress = serverOptions.LadderServer,
            ConnectionServerPort = serverOptions.GamePort,
            ConnectionClientPort = serverOptions.GamePort,
            MultiplayerSharedPort = serverOptions.GamePort,

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