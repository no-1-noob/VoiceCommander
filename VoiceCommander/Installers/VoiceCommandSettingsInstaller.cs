using VoiceCommander.Mgr;
using Zenject;

namespace VoiceCommander.Installers
{
    internal class VoiceCommandSettingsInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<VoiceCommandSettingsMgr>().AsSingle().NonLazy();
        }
    }
}
