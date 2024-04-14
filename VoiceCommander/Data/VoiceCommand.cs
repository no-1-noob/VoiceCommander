using System;
using UnityEngine.Windows.Speech;

namespace VoiceCommander.Data
{
    public class VoiceCommand
    {
        private string defaultKeyword;
        private string customKeyword;
        private string identifier;
        private ConfidenceLevel confidenceLevel;
        private bool activate;
        private Action action;

        public VoiceCommand(string identifier, string defaultKeyword, ConfidenceLevel confidenceLevel, Action action)
        {
            this.identifier = identifier;
            this.defaultKeyword = defaultKeyword;
            this.confidenceLevel = confidenceLevel;
            this.action = action;
        }

        public VoiceCommand()
        {

        }

        public string ActualKeyword { get => string.IsNullOrEmpty(this.customKeyword) ? defaultKeyword : this.customKeyword; }
        public string DefaultKeyword { get => defaultKeyword; set => defaultKeyword = value; }
        public string CustomKeyword { get => customKeyword; set => customKeyword = value; }
        public string Identifier { get => identifier; set => identifier = value; }
        public ConfidenceLevel ConfidenceLevel { get => confidenceLevel; set => confidenceLevel = value; }
        public bool Activate { get => activate; set => activate = value; }
        public Action Action { get => action; set => action = value; }
    }
}
