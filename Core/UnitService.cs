using Core.Intel;
using Core.Terran;

namespace Core;

public class UnitService : IUnitService
{
    private readonly IIntelService _intelService;
    private readonly IMessageService _messageService;

    public UnitService(IMessageService messageService, IIntelService intelService)
    {
        _messageService = messageService;
        _intelService = intelService;
    }

    public void Train(UnitType unitType)
    {
        // TODO: zerg + toss
        if (!TerranDataHelpers.Producers.TryGetValue(unitType, out var producers))
            throw new NotImplementedException();

        var producer = producers.First();

        var structures = _intelService.GetStructures(producer.Type).Select(x => x.Tag);

        _messageService.Action(producer.Ability, structures);
    }
}

public interface IUnitService
{
    void Train(UnitType unitType);
}