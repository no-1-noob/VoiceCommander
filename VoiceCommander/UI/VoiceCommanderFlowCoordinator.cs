using BeatSaberMarkupLanguage;
using HMUI;
using VoiceCommander.UI.ViewController;

namespace VoiceCommander.UI
{
    class VoiceCommanderFlowCoordinator : FlowCoordinator
    {
        private static VoiceCommanderFlowCoordinator instance = null;
        private static SettingsLeftViewController settingsLeftView;
        private static SettingsMidViewController settingsMidView;
        private static SettingsRightViewController settingsRightView;
        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            instance = this;
            SetTitle("VoiceCommander");
            showBackButton = true;
            settingsLeftView = BeatSaberUI.CreateViewController<SettingsLeftViewController>();
            settingsMidView = BeatSaberUI.CreateViewController<SettingsMidViewController>();
            settingsRightView = BeatSaberUI.CreateViewController<SettingsRightViewController>();
            ProvideInitialViewControllers(leftScreenViewController: settingsLeftView, mainViewController: settingsMidView,  rightScreenViewController: settingsRightView);
        }

        protected override void BackButtonWasPressed(HMUI.ViewController topViewController) => Close();

        private void Close()
        {
            Plugin.ParentFlow.DismissFlowCoordinator(instance, () => {
                instance = null;
            }, immediately: true);
        }
    }
}
