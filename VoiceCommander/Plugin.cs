using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using HMUI;
using IPA;
using IPA.Config.Stores;
using IPA.Utilities;
using SimpleUpdateChecker.Plugin;
using SiraUtil.Zenject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using VoiceCommander.Configuration;
using VoiceCommander.FeatureDefinition;
using VoiceCommander.Installers;
using VoiceCommander.UI;
using IPALogger = IPA.Logging.Logger;

namespace VoiceCommander
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    internal class Plugin : SimpleUpdatePlugin
    {
        internal static Plugin Instance { get; private set; }

        internal static List<VoiceCommandFeatureSettings> AvailableVoiceCommandFeatureSettings = new List<VoiceCommandFeatureSettings>();

        internal static FlowCoordinator ParentFlow { get; private set; }
        private static VoiceCommanderFlowCoordinator _flow;
        static Process _webSocketVoiceCommandRecognizerProcess;

        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="zenjector"></param>
        /// <param name="conf"></param>
        [Init]
        public void Init(IPALogger logger, Zenjector zenjector, IPA.Config.Config conf)
        {
            base.CreateSimpleUpdateChecker(logger, zenjector, "https://mods.no1noob.net/api/VoiceCommander_1_29", "https://github.com/no-1-noob/VoiceCommander/releases");
            Instance = this;
            if(conf != null)
            {
                PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
                PluginConfig.Instance.Changed();
            }

            MenuButtons.instance.RegisterButton(new MenuButton("Voice Commander", "Controll stuff with your voice", ShowSettingsFlow, true));
            foreach (Location location in (Location[])Enum.GetValues(typeof(Location)))
            {
                //Log.Error($"Installing to {location}");
                zenjector.Install<VoiceCommandInstaller>(location, location);
            }

            try
            {
                string path = Path.Combine(UnityGame.PluginsPath, "VCR\\VoiceCommandRecognizer.exe");
                if (File.Exists(path))
                {
                    _webSocketVoiceCommandRecognizerProcess = new Process();
                    _webSocketVoiceCommandRecognizerProcess.StartInfo.FileName = "cmd";
                    _webSocketVoiceCommandRecognizerProcess.StartInfo.Arguments = $"/C \"{path}\"";
                    _webSocketVoiceCommandRecognizerProcess.Start();
                }
                else
                {
                    Plugin.Log.Error($"Can't find VoiceCommandRecognizer in '{path}'");
                }
            }
            catch (Exception e)
            {
                Plugin.Log.Error($"Error starting VoiceCommandRecognizer process {e.Message}");
            }
        }

        [OnStart]
        public void OnApplicationStart()
        {
        }

        [OnExit]
        new public void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            try
            {
                //Fcukking kill the voice recognition when closing BS
                foreach (var voiceCommandRecognizerProcess in Process.GetProcessesByName("VoiceCommandRecognizer"))
                {
                    voiceCommandRecognizerProcess.Kill();
                }
                _webSocketVoiceCommandRecognizerProcess?.CloseMainWindow();
                _webSocketVoiceCommandRecognizerProcess.WaitForExit();
            }
            catch (Exception ex)
            {
                Log.Error($"OnApplicationQuit Error {ex.Message}");
                throw;
            }
        }

        private static void ShowSettingsFlow()
        {
            if (_flow == null)
                _flow = BeatSaberUI.CreateFlowCoordinator<VoiceCommanderFlowCoordinator>();

            ParentFlow = BeatSaberUI.MainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf();

            BeatSaberUI.PresentFlowCoordinator(ParentFlow, _flow, immediately: true);
        }
    }
}
