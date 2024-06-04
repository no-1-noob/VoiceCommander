using SiraUtil.Zenject;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using VoiceCommander.Configuration;
using VoiceCommander.Data;
using VoiceCommander.Interfaces;
using VoiceCommander.Utils;
using Zenject;

namespace VoiceCommander.Mgr
{
    internal class CoreMgr : IInitializable, IDisposable
    {
        private const string MainMenu = "MainMenu";
        [Inject] protected List<IVoiceCommandHandler> lsVoiceCommand = new List<IVoiceCommandHandler>();

        private List<VoiceCommand> lsKeyWordActions = new List<VoiceCommand>();
        private IRecognizer recognizer;
        private Location location;
        private bool active = false;

        public CoreMgr(Location location)
        {
            this.location = location;
        }

        public void Initialize()
        {
            active = true;
            SetupAndCheckKeyWords();
            CreateRecognizer();
            VCEventHandler.OnSettingsChanged += OnSettingsChanged;
            VCEventHandler.OnSettingsRecognizerStateChange += OnSettingsRecognizerStateChange;
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        public void Dispose()
        {
            active = false;
            lsKeyWordActions.Clear();
            if (recognizer != null) recognizer.OnKeyWordRecognized -= Recognizer_keyWordRecognized;
            recognizer?.CloseAndCleanupRecognizer();
            VCEventHandler.OnSettingsChanged -= OnSettingsChanged;
            VCEventHandler.OnSettingsRecognizerStateChange -= OnSettingsRecognizerStateChange;
        }

        /// <summary>
        /// Special Scene change detection for Main menu, as it is not destroyed while changing to gameplay
        /// </summary>
        /// <param name="prevScene"></param>
        /// <param name="currentScene"></param>
        private void SceneManager_activeSceneChanged(Scene prevScene, Scene currentScene)
        {
            if (location.HasFlag(Location.Menu))
            {
                if (active && prevScene.name == MainMenu && currentScene.name != MainMenu)
                {
                    Dispose();
                }
                if (!active && prevScene.name != MainMenu && currentScene.name == MainMenu)
                {
                    Initialize();
                }
            }
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            //Reload Settings and restart VoiceRecognition
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
