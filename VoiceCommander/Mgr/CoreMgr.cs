using System;
using System.Collections.Generic;
using System.Linq;
using VoiceCommander.Configuration;
using VoiceCommander.Data;
using VoiceCommander.Interfaces;
using VoiceCommander.Recognizer;
using VoiceCommander.Utils;
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
            VCEventHandler.OnSettingsChanged += OnSettingsChanged;
            VCEventHandler.OnSettingsRecognizerStateChange += OnSettingsRecognizerStateChange;
        }

        public void OnSettingsChanged(object sender, EventArgs e)
        {
            //Reload Settings and restart VoiceRecognition
            Plugin.Log.Error($"Reload Settings and restart VoiceRecognition");
            foreach (var item in lsKeyWordActions)
            {
                PluginConfig.Instance.ApplySettingsToVoiceCommand(item);
            }
            recognizer.ResetRecognizer(lsKeyWordActions.Select(x => x.ActualKeyword).ToList());
        }

        private void OnSettingsRecognizerStateChange(object sender, bool isActive)
        {
            if (isActive) recognizer.Pause();
            else recognizer.Start();
        }

        private void SetupAndCheckKeyWords()
        {
            foreach (var vcommand in lsVoiceCommand.SelectMany(voiceCommandPlugin => voiceCommandPlugin.lsVoicecommand))
            {
                lsKeyWordActions.Add(vcommand);
            }
        }

        public void Dispose()
        {
            if(recognizer != null) recognizer.OnKeyWordRecognized -= Recognizer_keyWordRecognized;
            recognizer?.CloseAndCleanupRecognizer();
            VCEventHandler.OnSettingsChanged -= OnSettingsChanged;
            VCEventHandler.OnSettingsRecognizerStateChange -= OnSettingsRecognizerStateChange;
        }

        private void CreateRecognizer()
        {
            recognizer = Utils.Utils.CreateRecognizer(lsKeyWordActions, Recognizer_keyWordRecognized);
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
                voiceCommand.Activate = confidence >= voiceCommand.ActualConfidenceLevel;
            }
            return lsVoiceCommand;
        }
    }
}
