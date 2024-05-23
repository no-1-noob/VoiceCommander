using System;

namespace VoiceCommander.Utils
{

    internal class VCEventHandler
    {
        public static EventHandler OnSettingsChanged;
        public static EventHandler<bool> OnSettingsRecognizerStateChange;
    }
}
