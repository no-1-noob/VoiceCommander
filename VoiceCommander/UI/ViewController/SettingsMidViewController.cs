using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceCommander.UI.ViewController
{
    [HotReload(RelativePathToLayout = @"SettingsMidViewController.bsml")]
    [ViewDefinition("VoiceCommander.UI.View.SettingsMidView.bsml")]
    internal class SettingsMidViewController : BSMLAutomaticViewController, INotifyPropertyChanged
    {
    }
}
