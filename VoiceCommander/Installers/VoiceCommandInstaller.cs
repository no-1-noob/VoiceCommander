using SiraUtil.Zenject;
using System.Collections.Generic;
using System.Linq;
using VoiceCommander.FeatureDefinition;
using VoiceCommander.Mgr;
using Zenject;

namespace VoiceCommander.Installers
{
    internal class VoiceCommandInstaller : Installer
    {
        List<VoiceCommandFeatureSettings> voiceCommandFeaturesToInstall;
        Location _intallLocation;

        public VoiceCommandInstaller(Location installLocation)
        {
            _intallLocation = installLocation;
            this.voiceCommandFeaturesToInstall = Plugin.AvailableVoiceCommandFeatureSettings.Where(x => (x.ZenjectLocationEnum.HasFlag(installLocation))).ToList();
        }
        public override void InstallBindings()
        {
            if(voiceCommandFeaturesToInstall?.Count > 0)
            {
                CreateVoiceCommandHandler();
                Container.BindInterfacesAndSelfTo<CoreMgr>().AsSingle().WithArguments(_intallLocation).NonLazy();
            }
        }

        private void CreateVoiceCommandHandler()
        {
            foreach (VoiceCommandFeatureSettings featureSetting in voiceCommandFeaturesToInstall)
            {
                Container.BindInterfacesAndSelfTo(featureSetting.CommandType).AsSingle().NonLazy();
            }
        }
    }
}
