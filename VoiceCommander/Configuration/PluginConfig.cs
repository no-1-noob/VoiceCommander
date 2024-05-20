
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using Newtonsoft.Json;
using VoiceCommander.Data;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace VoiceCommander.Configuration
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }

        //For some reason lists are not serialized automatically so use this hack for now
        public virtual string customKeywords { get; set; } = string.Empty;

        private List<VoiceCommand> lsVoiceCommandSettings = new List<VoiceCommand>();

        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            // Do stuff after config is read from disk.
            if (string.IsNullOrEmpty(customKeywords))
            {
                customKeywords = JsonConvert.SerializeObject(new List<VoiceCommand>());
            }
            lsVoiceCommandSettings = JsonConvert.DeserializeObject<List<VoiceCommand>>(customKeywords);
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            // Do stuff when the config is changed.
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
        }

        internal void UpdateKeyWord(VoiceCommand command)
        {
            var existingVoiceCommandSetting = lsVoiceCommandSettings.Find(x => x.Identifier == command.Identifier);
            if (existingVoiceCommandSetting == null)
            {
                lsVoiceCommandSettings.Add(command);
            }
            else
            {
                existingVoiceCommandSetting.ApplySavedCustomKeywordSettings(command);
            }
            customKeywords = JsonConvert.SerializeObject(lsVoiceCommandSettings);
        }

        internal void ResetKeyWord(VoiceCommand command)
        {
            command.ResetCustomSettings();
            var existingVoiceCommandSetting = lsVoiceCommandSettings.Find(x => x.Identifier == command.Identifier);
            if (existingVoiceCommandSetting != null)
            {
                existingVoiceCommandSetting.ResetCustomSettings();
            }
            customKeywords = JsonConvert.SerializeObject(lsVoiceCommandSettings);
        }

        internal void ApplySettingsToVoiceCommand(VoiceCommand command)
        {
            VoiceCommand existingVoiceCommandSetting = lsVoiceCommandSettings.Find(x => x.Identifier == command.Identifier);
            if(existingVoiceCommandSetting != null)
            {
                command.ApplySavedCustomKeywordSettings(existingVoiceCommandSetting);
            }
        }
    }
}
