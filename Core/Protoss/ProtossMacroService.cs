using Core.Intel;
using SC2APIProtocol;

namespace Core.Protoss;

public class ProtossMacroService : MacroService
{
    public ProtossMacroService(IMessageService messageService, IEnumerable<IIntelService> intelServices) : base(messageService, intelServices)
    {
    }

    public override Race Race => Race.Protoss;
}