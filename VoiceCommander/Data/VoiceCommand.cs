using Newtonsoft.Json;
using System;
using System.Reflection;

namespace VoiceCommander.Data
{
    /// <summary>
    /// Voice command definition for use with VoiceCommander BS mod
    /// </summary>
    public class VoiceCommand
    {
        private string _defaultKeyword;
        private string _customKeyword;
        private string _identifier;
        private float _defaultConfidenceLevel;
        private float? _customConfidenceLevel;
        private bool activate;
        private Action _action;
        private string _modName;

        /// <summary>
        /// Returns the custom keyword for voice recognition if set, otherwise the default keyword
        /// </summary>
        public string ActualKeyword { get => string.IsNullOrEmpty(this._customKeyword) ? _defaultKeyword : this._customKeyword; }
        /// <summary>
        /// Default keyword to be used in voice recognition as intended by the VoiceCommand creator
        /// </summary>
        public string DefaultKeyword { get => _defaultKeyword; set => _defaultKeyword = value; }
        /// <summary>
        /// CustomKeyword keyword to be used in voice recognition. Set at runtime in the VoiceCommander mod
        /// </summary>
        public string CustomKeyword { get => _customKeyword; set => _customKeyword = value; }
        /// <summary>
        /// Identifier for this Keyword. Please use namespace-like names to avoid collisions (e.g. PauseCommander.PauseCommandMainMenu.MyKeyWord)
        /// </summary>
        public string Identifier { get => _identifier; set => _identifier = value; }
        /// <summary>
        /// Default Value for how  confident does the speech recognition need to be in order to trigger the keyword (0->1 with 1 beign perfect)
        /// </summary>
        public float ActualConfidenceLevel { get => _customConfidenceLevel ?? _defaultConfidenceLevel; }
        /// <summary>
        /// Default Value for how  confident does the speech recognition need to be in order to trigger the keyword (0->1 with 1 beign perfect)
        /// </summary>
        public float DefaultConfidenceLevel { get => _defaultConfidenceLevel; set => _defaultConfidenceLevel = value; }
        /// <summary>
        /// Custom Value for how confident does the speech recognition need to be in order to trigger the keyword (0->1 with 1 beign perfect)
        /// </summary>
        public float? CustomConfidenceLevel { get => _customConfidenceLevel; set => _customConfidenceLevel = value; }
        /// <summary>
        /// Should the action be executed. Set when the voice recognition has enough confidence
        /// </summary>
        public bool Activate { get => activate; set => activate = value; }
        /// <summary>
        /// Action to execute when the keyword is triggered
        /// </summary>
        [JsonIgnore]
        public Action Action { get => _action; set => _action = value; }
        /// <summary>
        /// Name of the mod that created this VoiceCommand
        /// </summary>
        public string ModName { get => _modName; }

        /// <summary>
        /// Create a new VoiceCommand
        /// </summary>
        /// <param name="identifier">Identifier for this Keyword. Please use namespace-like names to avoid collisions (e.g. PauseCommander.PauseCommandMainMenu.MyKeyWord)</param>
        /// <param name="defaultKeyword">Default keyword to be used in voice recognition as intended by the VoiceCommand creator</param>
        /// <param name="confidenceLevel">How confident does the speech recognition need to be in order to trigger the keyword (0->1 with 1 beign perfect)</param>
        /// <param name="action">Action to execute when the keyword is triggered</param>
        public VoiceCommand(string identifier, string defaultKeyword, float confidenceLevel, Action action)
        {
            if (string.IsNullOrEmpty(identifier)) throw new ArgumentNullException("identifier");
            if (string.IsNullOrEmpty(defaultKeyword)) throw new ArgumentNullException("defaultKeyword");
            if (confidenceLevel < 0 || confidenceLevel > 1) throw new ArgumentOutOfRangeException("confidenceLevel");
            if (action == null) throw new ArgumentNullException("action");
            this._identifier = identifier;
            this._defaultKeyword = defaultKeyword;
            this._defaultConfidenceLevel = confidenceLevel;
            this._action = action;
            this._modName = Assembly.GetCallingAssembly().GetName().Name;
            Configuration.PluginConfig.Instance.ApplySettingsToVoiceCommand(this);
        }

        [JsonConstructor]
        internal VoiceCommand()
        {

        }

        internal void ApplySavedCustomKeywordSettings(VoiceCommand savedCustomSettings)
        {
            this._customKeyword = savedCustomSettings._customKeyword;
            this._customConfidenceLevel = savedCustomSettings.CustomConfidenceLevel;
        }

        internal void ResetCustomSettings()
        {
            this._customKeyword = string.Empty;
            this._customConfidenceLevel = null;
        }

        /// <summary>
        /// Well a to string... show some infos ;)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ActualKeyword;
            //return $"Mod: {ModName} Identifier: '{Identifier}' ActualKeyword: '{ActualKeyword}', Confidence: '{DefaultConfidenceLevel}'";
        }
    }
}
