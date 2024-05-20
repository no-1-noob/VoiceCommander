using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceCommander.Data;
using VoiceCommander.Interfaces;
using VoiceCommander.Recognizer;
using static VoiceCommander.Utils.Enum;

namespace VoiceCommander.Utils
{
    internal class Utils
    {
        internal static IRecognizer CreateRecognizer(List<VoiceCommand> lsKeyWordActions, EventHandler<(string, float)> Recognizer_keyWordRecognized)
        {
            IRecognizer recognizer;
            VoiceRecognizerType type = VoiceRecognizerType.ExternalWebSocketSystemSpeechRecognizer;
            switch (type)
            {
                case VoiceRecognizerType.ExternalWebSocketSystemSpeechRecognizer:
                    recognizer = new ExternalWebSocketSystemSpeechRecognizer();
                    break;
                default:
                    Plugin.Log.Error($"The VoiceRecognizerType {type} could not be parsed;");
                    return null;
            }
            recognizer.CreateAndStartRecognizer(lsKeyWordActions.Select(x => x.ActualKeyword).ToList());
            recognizer.OnKeyWordRecognized += Recognizer_keyWordRecognized;

            return recognizer;
        }
    }
}
