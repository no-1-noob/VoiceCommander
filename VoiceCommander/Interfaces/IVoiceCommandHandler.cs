using System.Collections.Generic;
using VoiceCommander.Data;

namespace VoiceCommander.Interfaces
{
    public interface IVoiceCommandHandler
    {
        List<VoiceCommand> lsVoicecommand { get; }
    }
}
