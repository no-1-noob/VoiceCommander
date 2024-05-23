using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.ComponentModel;
using VoiceCommander.Interfaces;

namespace VoiceCommander.UI.ViewController
{
    [HotReload(RelativePathToLayout = @"SettingsRightViewController.bsml")]
    [ViewDefinition("VoiceCommander.UI.View.SettingsRightView.bsml")]
    internal class SettingsRightViewController : BSMLAutomaticViewController, INotifyPropertyChanged
    {
        public new event PropertyChangedEventHandler PropertyChanged;

        private string _textActivate = "Activate Test";
        private string _textDeactivate = "Deactivate Test";
        private string _recognizerAction = "Activate Test";
        private bool _isActive = false;

        private IRecognizer recognizer;

        private string _recognizedText = string.Empty;
        private string _recognizedConfidence = string.Empty;
        private string _recognizedTimestamp = string.Empty;

        [UIAction("click-toggle-test-recognizer")]
        private void ClickToggleTestRecognizer()
        {
            if (!_isActive)
            {
                RecognizerAction = _textDeactivate;
                if (recognizer == null)
                {
                    recognizer = Utils.Utils.CreateRecognizer(Utils.Utils.GetAllVocieCommands(), Recognizer_keyWordRecognized);
                }
                else
                {
                    recognizer.Start();
                }
            }
            else
            {
                RecognizerAction = _textActivate;
                recognizer.Pause();
            }
            _isActive = !_isActive;
            Utils.VCEventHandler.OnSettingsRecognizerStateChange?.Invoke(this, _isActive);
        }

        private void Recognizer_keyWordRecognized(object sender, (string keyWord, float confidence) v)
        {
            RecognizedText = v.keyWord;
            RecognizedConfidence = v.confidence.ToString("N4");
            RecognizedTimestamp = DateTime.Now.ToString("T");
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            if (_isActive)
            {
                recognizer?.CloseAndCleanupRecognizer();
                Utils.VCEventHandler.OnSettingsRecognizerStateChange?.Invoke(this, false);
            }

            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        }

        [UIValue("recognizer-action")]
        public string RecognizerAction
        {
            get => _recognizerAction;
            set
            {
                _recognizerAction = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RecognizerAction)));
            }
        }

        [UIValue("recognized-text")]
        public string RecognizedText
        {
            get => _recognizedText;
            set
            {
                _recognizedText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RecognizedText)));
            }
        }

        [UIValue("recognized-confidence")]
        public string RecognizedConfidence
        {
            get => _recognizedConfidence; set
            {
                _recognizedConfidence = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RecognizedConfidence)));
            }
        }

        [UIValue("recognized-timestamp")]
        public string RecognizedTimestamp
        {
            get => _recognizedTimestamp; set
            {
                _recognizedTimestamp = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RecognizedTimestamp)));
            }
        }
    }
}
