using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.ComponentModel;
using VoiceCommander.Configuration;

namespace VoiceCommander.UI.ViewController
{
    [HotReload(RelativePathToLayout = @"SettingsLeftViewController.bsml")]
    [ViewDefinition("VoiceCommander.UI.View.SettingsLeftView.bsml")]
    internal class SettingsLeftViewController : BSMLAutomaticViewController, INotifyPropertyChanged
    {
        public new event PropertyChangedEventHandler PropertyChanged;

        [UIValue("current-port")]
        public string CurrentPort
        {
            get => PluginConfig.Instance.Port.ToString();
            set
            {
                if (Int32.TryParse(value, out int parsedPort))
                {
                    PluginConfig.Instance.Port = parsedPort;
                }
                else
                {
                    PluginConfig.Instance.Port = Utils.Utils.DefaultPort;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentPort)));
            }
        }
    }
}
