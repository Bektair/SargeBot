using Core.Bot;
using Microsoft.Extensions.DependencyInjection;
using SargeBot.Features.Macro.Build;
using SC2APIProtocol;

namespace SargeBot;

public class SargeBot : ZergBot
{
    private readonly Build _build;

    public SargeBot(IServiceProvider services) : base(services)
    {
        _build = services.GetRequiredService<Build>();
    }


    //This object has a state machine like thing to make sure we act on the right code each frame
    //Context: defines the interface of intrest, maintains an instance of a concretestate subclass that defines the current state
    //State: defines an interface for encapsulating the behavior associated with the particular state of the context
    //Concrete State: each subclass implements a behavior associated with a state of context

    //States: ZerglingRush, Droning, MutaMan! Clear the queues when change state maby

    public override void OnStart(ResponseObservation firstObservation, ResponseData responseData, ResponseGameInfo gameInfo)
    {
        base.OnStart(firstObservation, responseData, gameInfo);
        /*
        _staticGameData.PopulateGameData(responseData);
        Task.Run(() => _staticGameData.Save());
        _mapService.PopulateMapData(gameInfo);
        //The starting build needs 
        */
    }

    public override void OnFrame(ResponseObservation observation)
    {
        base.OnFrame(observation);

        _build.State.NewObservations(observation);
        _build.State.ExecuteBuild();
        //TODO: put MessageService.Action() in the builds
    }
}