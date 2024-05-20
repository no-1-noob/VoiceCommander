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
    [HotReload(RelativePathToLayout = @"SettingsRightViewController.bsml")]
    [ViewDefinition("VoiceCommander.UI.View.SettingsRightView.bsml")]
    internal class SettingsRightViewController : BSMLAutomaticViewController, INotifyPropertyChanged
    {
    }
}
