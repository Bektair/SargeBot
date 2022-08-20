using SargeBot.Features.GameData;
using SC2APIProtocol;
using SC2ClientApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.Macro.ProductionQueue
{

  /// <summary>
  /// Listens to the UnitProductionQueues larvaQueue
  /// </summary>
  public class LarvaQueue
  {

    StaticGameData staticGameData;
    public Queue<UnitType> larvaQueue = new Queue<UnitType>();

    public LarvaQueue(StaticGameData staticGameData)
    {
      this.staticGameData = staticGameData;
    }

    public bool CanCreate(ResponseObservation obs)
    {
      //I should make a static function somewere that takes in observation and unit and tells you if it is possible to create
      PlainUnit unit = staticGameData.PlainUnits[larvaQueue.Peek()];
      uint minerals = obs.Observation.PlayerCommon.Minerals;

      return HasLarvae(obs) && IProduceable.CanCreate(obs, unit);
    }

    public int CountLarvae(ResponseObservation obs)
    {
      return obs.Observation.RawData.Units.Count(u => u.UnitType.Is(UnitType.ZERG_LARVA));
    }

    public int EggsOfUnitType(ResponseObservation obs)
    {
      var eggs = obs.Observation.RawData.Units.Where(u => u.UnitType.Is(UnitType.ZERG_EGG));
      //eggs.Where(egg => egg
      return 0;
    }

    private bool HasLarvae(ResponseObservation obs)
    {
      return CountLarvae(obs) > 0;
    }

    public void Enqueue(UnitType unitType)
    {
      larvaQueue.Enqueue(unitType);
    }

    public UnitType Dequeue()
    {
      return larvaQueue.Dequeue();
    }


    /// <summary>
    /// Should return a object with only the cost
    /// Can be called after IsEmpty to see if you are able to afford it
    /// Impossible to afford if there is a empty queue
    /// </summary>
    /// <returns></returns>
    public IProduceable PeekLarva()
    {
        return staticGameData.PlainUnits[larvaQueue.Peek()];
    }

    public bool IsEmpty()
    {
      return Count()==0;
    }

    /// <summary>
    /// If none then why bother
    /// </summary>
    /// <returns></returns>
    public int Count()
    {
      return larvaQueue.Count;
    }





  }
}
