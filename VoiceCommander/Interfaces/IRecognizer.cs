using System;
using System.Collections.Generic;

namespace VoiceCommander.Interfaces
{
    internal interface IRecognizer
    {
        void CreateAndStartRecognizer(List<string> lsKeywords);
        void CloseAndCleanupRecognizer();

        event EventHandler<(string, float)> OnKeyWordRecognized;
    }
}
