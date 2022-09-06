using Core.Intel;
using SC2APIProtocol;

namespace Core.Terran;

public class TerranMacroService : MacroService
{
    public TerranMacroService(IMessageService messageService, IEnumerable<IIntelService> intelServices) : base(messageService, intelServices)
    {
    }

    public override Race Race => Race.Terran;
}