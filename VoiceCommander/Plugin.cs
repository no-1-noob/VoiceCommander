using IPA;
using SiraUtil.Zenject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using VoiceCommander.Installers;
using IPALogger = IPA.Logging.Logger;

namespace VoiceCommander
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        internal static List<VoiceCommandSettings> AvailableVoiceCommandSettings = new List<VoiceCommandSettings>();

        static Process _webSocketVoiceCommandRecognizerProcess;

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger, Zenjector zenjector)
        {
            Instance = this;
            Log = logger;
            Log.Info("VoiceCommander initialized.");
            foreach (Location location in (Location[])Enum.GetValues(typeof(Location)))
            {
                Log.Error($"Installing to {location}");
                zenjector.Install<VoiceCommandInstaller>(location, location);
            }

            try
            {
                string path = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Beat Saber Dev\\1_29\\Plugins\\VCR\\VoiceCommandRecognizer.exe";
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
                Plugin.Log.Error($"Process {e.Message}");
            }
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");

        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Error($"OnApplicationQuit {_webSocketVoiceCommandRecognizerProcess}");
            //Fcukking kill the voice recognition when closing BS
            foreach (var voiceCommandRecognizerProcess in Process.GetProcessesByName("VoiceCommandRecognizer"))
            {
                voiceCommandRecognizerProcess.Kill();
            }
            _webSocketVoiceCommandRecognizerProcess?.CloseMainWindow();
            _webSocketVoiceCommandRecognizerProcess.WaitForExit();
        }
    }
}
