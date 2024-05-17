using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoiceCommander.Data;
using VoiceCommander.Interfaces;
using VoiceCommander.Recognizer;
using WebSocketSharp;
using Zenject;

namespace VoiceCommander.Mgr
{
    internal class CoreMgr : IInitializable, IDisposable
    {
        [Inject] protected List<IVoiceCommandHandler> lsVoiceCommand = new List<IVoiceCommandHandler>();

        private List<VoiceCommand> lsKeyWordActions = new List<VoiceCommand>();
        private IRecognizer recognizer;

        public void Initialize()
        {
            SetupAndCheckKeyWords();
            CreateRecognizer();
        }

        private void SetupAndCheckKeyWords()
        {
            foreach (var vcommand in lsVoiceCommand.SelectMany(voiceCommandPlugin => voiceCommandPlugin.lsVoicecommand))
            {
                if (lsKeyWordActions.Exists(x => x.Identifier == vcommand.Identifier))
                {
                    Plugin.Log.Error($"Voicecommand with Identifier '{vcommand.Identifier}' was not registered as another with the same identifier was already registered");
                    continue;
                }

                lsKeyWordActions.Add(vcommand);
            }
        }

        public void Dispose()
        {
            Plugin.Log.Error($"Dispose coreMgr");
            if(recognizer != null) recognizer.OnKeyWordRecognized -= Recognizer_keyWordRecognized;
            recognizer?.CloseAndCleanupRecognizer();
        }

        private void CreateRecognizer()
        {
            recognizer = new ExternalWebSocketSystemSpeechRecognizer();
            recognizer.CreateAndStartRecognizer(lsKeyWordActions.Select(x => x.ActualKeyword).ToList());
            recognizer.OnKeyWordRecognized += Recognizer_keyWordRecognized;
        }

        private void Recognizer_keyWordRecognized(object sender, (string keyWord, float confidence) v)
        {
            List<VoiceCommand> lsVoiceCommand = ParseActions(v.keyWord, v.confidence);
            foreach (var vcommand in lsVoiceCommand)
            {
                if (vcommand.Activate)
                {
                    try
                    {
                        vcommand.Action.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Plugin.Log.Error($"Error while activating Action for Keyword '{vcommand.Identifier}': {ex.Message}");
                    }
                }
            }
        }

        private List<VoiceCommand> ParseActions(string keyword, float confidence)
        {
            List<VoiceCommand> lsVoiceCommand = lsKeyWordActions.Where(x => x.DefaultKeyword.ToLower() == keyword.ToLower()).ToList();
            foreach (VoiceCommand voiceCommand in lsVoiceCommand)
            {
                voiceCommand.Activate = confidence >= voiceCommand.ConfidenceLevel;
            }
            return lsVoiceCommand;
        }
    }
}
