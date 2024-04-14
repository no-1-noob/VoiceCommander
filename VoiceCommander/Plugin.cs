using IPA;
using SiraUtil.Zenject;
using System;
using System.Collections.Generic;
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
            //zenjector.Install<CoreInstaller>(Location.App);

            foreach (Location location in (Location[])Enum.GetValues(typeof(Location)))
            {
                Log.Error($"Installing to {location}");
                zenjector.Install<VoiceCommandInstaller>(location, location);
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
            Log.Debug("OnApplicationQuit");

        }
    }
}
