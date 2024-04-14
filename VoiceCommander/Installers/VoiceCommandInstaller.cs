using SiraUtil.Zenject;
using System.Collections.Generic;
using System.Linq;
using VoiceCommander.Mgr;
using Zenject;

namespace VoiceCommander.Installers
{
    internal class VoiceCommandInstaller : Installer
    {
        List<VoiceCommandSettings> voiceCommandsToInstall;

        public VoiceCommandInstaller(Location intallLocation)
        {
            this.voiceCommandsToInstall = Plugin.AvailableVoiceCommandSettings.Where(x => x.ZenjectLocationEnum == intallLocation).ToList();
        }
        public override void InstallBindings()
        {
            if(voiceCommandsToInstall?.Count > 0)
            {
                CreateVoiceCommandHandler();
                Container.BindInterfacesAndSelfTo<CoreMgr>().AsSingle().NonLazy();
            }
        }

        private void CreateVoiceCommandHandler()
        {
            foreach (VoiceCommandSettings voiceCommand in voiceCommandsToInstall)
            {

                Plugin.Log.Error($"Loading voice command {voiceCommand.Name} {voiceCommand.CommandType} {voiceCommand.CommandType?.BaseType?.FullName}...");
                Container.BindInterfacesAndSelfTo(voiceCommand.CommandType).AsSingle().NonLazy();
            }
        }
    }
}
