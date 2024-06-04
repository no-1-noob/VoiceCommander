using Newtonsoft.Json;
using SiraUtil.Zenject;
using System;
using System.Collections.Generic;

namespace VoiceCommander.FeatureDefinition
{
    internal class VoiceCommandSettingsList
    {
        [JsonProperty(Required = Required.Always)]
        public List<VoiceCommandFeatureSettings> Commands;

    }

    internal class VoiceCommandFeatureSettings
    {
        [JsonProperty(Required = Required.Always)]
        public string Name;

        [JsonProperty(Required = Required.Always)]
        public string CommandLoacation;

        [JsonProperty(Required = Required.Always)]
        public string ZenjectLocation;

        public Type CommandType;

        public Location ZenjectLocationEnum;
    }
}
