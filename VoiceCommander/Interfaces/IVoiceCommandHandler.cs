using System.Collections.Generic;
using VoiceCommander.Data;

namespace VoiceCommander.Interfaces
{
    /// <summary>
    /// Interface to handle voice commands from other mods
    /// </summary>
    public interface IVoiceCommandHandler
    {
        /// <summary>
        /// List of all available voice commands for this Handler
        /// </summary>
        List<VoiceCommand> lsVoicecommand { get; }
    }
}
