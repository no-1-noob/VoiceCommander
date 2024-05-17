using System;

namespace VoiceCommander.Data
{
    public class VoiceCommand
    {
        private string defaultKeyword;
        private string customKeyword;
        private string identifier;
        private float confidenceLevel;
        private bool activate;
        private Action action;

        /// <summary>
        /// Create a new VoiceCommand
        /// </summary>
        /// <param name="identifier">Default keyword to be used in voice recognition as intended by the VoiceCommand creator</param>
        /// <param name="defaultKeyword">Identifier for this Keyword. Please use namespace-like names to avoid collisions (e.g. PauseCommander.PauseCommandMainMenu.MyKeyWord)</param>
        /// <param name="confidenceLevel">How confident does the speech recognition need to be in order to trigger the keyword (0->1 with 1 beign perfect)</param>
        /// <param name="action">Action to execute when the keyword is triggered</param>
        public VoiceCommand(string identifier, string defaultKeyword, float confidenceLevel, Action action)
        {
            this.identifier = identifier;
            this.defaultKeyword = defaultKeyword;
            this.confidenceLevel = confidenceLevel;
            this.action = action;
        }

        /// <summary>
        /// Returns the custom keyword for voice recognition if set, otherwise the default keyword
        /// </summary>
        public string ActualKeyword { get => string.IsNullOrEmpty(this.customKeyword) ? defaultKeyword : this.customKeyword; }
        /// <summary>
        /// Default keyword to be used in voice recognition as intended by the VoiceCommand creator
        /// </summary>
        public string DefaultKeyword { get => defaultKeyword; set => defaultKeyword = value; }
        /// <summary>
        /// CustomKeyword keyword to be used in voice recognition. Set at runtime in the VoiceCommander mod
        /// </summary>
        public string CustomKeyword { get => customKeyword; set => customKeyword = value; }
        /// <summary>
        /// Identifier for this Keyword. Please use namespace-like names to avoid collisions (e.g. PauseCommander.PauseCommandMainMenu.MyKeyWord)
        /// </summary>
        public string Identifier { get => identifier; set => identifier = value; }
        /// <summary>
        /// How confident does the speech recognition need to be in order to trigger the keyword (0->1 with 1 beign perfect)
        /// </summary>
        public float ConfidenceLevel { get => confidenceLevel; set => confidenceLevel = value; }
        /// <summary>
        /// Should the action be executed. Set when the voice recognition has enough confidence
        /// </summary>
        public bool Activate { get => activate; set => activate = value; }
        /// <summary>
        /// Action to execute when the keyword is triggered
        /// </summary>
        public Action Action { get => action; set => action = value; }
    }
}
