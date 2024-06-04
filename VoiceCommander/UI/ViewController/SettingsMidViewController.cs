using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using VoiceCommander.Configuration;
using VoiceCommander.Data;
using VoiceCommander.Utils;

namespace VoiceCommander.UI.ViewController
{
    [HotReload(RelativePathToLayout = @"SettingsMidViewController.bsml")]
    [ViewDefinition("VoiceCommander.UI.View.SettingsMidView.bsml")]
    internal class SettingsMidViewController : BSMLAutomaticViewController, INotifyPropertyChanged
    {
        public new event PropertyChangedEventHandler PropertyChanged;
        private Dictionary<string, List<VoiceCommand>> _dctMod = new Dictionary<string, List<VoiceCommand>>();
        private List<object> _lsSettingCommand = new List<object>() { };
        private string _currentMod = string.Empty;
        private VoiceCommand _currentCommand = new VoiceCommand();

        public SettingsMidViewController()
        {
            List<VoiceCommand> lsAllCommands = Utils.Utils.GetAllVocieCommands();
            foreach (var mod in lsAllCommands.GroupBy(x => x.ModName))
            {
                _dctMod[mod.Key] = mod.Select(x => x).ToList();
            }
            CurrentMod = _dctMod.FirstOrDefault().Key;
        }

        [UIValue("all-mod-options")]
        public List<object> AllModOptions
        {
            get
            {
                return _dctMod.Keys.ToList<object>();
            }
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AllModOptions)));
            }
        }

        [UIValue("current-mod")]
        public object CurrentMod
        {
            get => _currentMod;
            set
            {
                _currentMod = value as string;
                if (_dctMod.TryGetValue(_currentMod, out List<VoiceCommand> lsVoiceCommands))
                {
                    _lsSettingCommand = lsVoiceCommands.ToList<object>();
                    _currentCommand = lsVoiceCommands.FirstOrDefault();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentMod)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AllCommandOptions)));
                    RefreshSelectedCommand();
                }
            }
        }

        [UIComponent("dropdown-commands")]
        public DropDownListSetting dropdownCommandController;

        [UIValue("all-command-options")]
        public List<object> AllCommandOptions
        {
            get
            {
                return _lsSettingCommand;
            }
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AllCommandOptions)));
            }
        }

        [UIValue("current-command")]
        public object CurrentCommand
        {
            get => _currentCommand;
            set
            {
                PluginConfig.Instance.ApplySettingsToVoiceCommand(_currentCommand); //Reload the previous selected from settings in case it was not saved
                _currentCommand = value as VoiceCommand;
                PluginConfig.Instance.ApplySettingsToVoiceCommand(_currentCommand);
                RefreshSelectedCommand();
            }
        }

        [UIValue("current-command-identifier")]
        public object CurrentCommandIdentifier
        {
            get
            {
                return _currentCommand.Identifier;
            }
        }

        [UIValue("current-command-keyword")]
        public string CurrentCommandKeyword
        {
            get
            {
                return _currentCommand.ActualKeyword;
            }
            set
            {
                _currentCommand.CustomKeyword = value;
                RefreshSelectedCommand();
            }
        }

        [UIValue("current-command-confidence")]
        public float CurrentCommandConfidence
        {
            get
            {
                return _currentCommand.ActualConfidenceLevel;
            }
            set
            {
                _currentCommand.CustomConfidenceLevel = value;
                RefreshSelectedCommand();
            }
        }

        [UIAction("click-save")]
        private void OnClickSave()
        {
            //Reload all currently running Recognizers (e.g main menu / app)
            PluginConfig.Instance.UpdateKeyWord(_currentCommand);
            RefreshSelectedCommand(true);
            VCEventHandler.OnSettingsChanged?.Invoke(this, null);
        }

        [UIAction("click-reset")]
        private void OnClickReset()
        {
            PluginConfig.Instance.ResetKeyWord(_currentCommand);
            RefreshSelectedCommand(true);
        }

        private void RefreshSelectedCommand(bool updateCommand = false)
        {
            if (updateCommand)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCommand)));
                RefreshCommandOptions();
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCommandIdentifier)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCommandKeyword)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCommandConfidence)));
        }

        private void RefreshCommandOptions()
        {
            if (dropdownCommandController)
            {
                dropdownCommandController.values = AllCommandOptions;
                dropdownCommandController.UpdateChoices();
                dropdownCommandController.ApplyValue();
            }
        }
    }
}
