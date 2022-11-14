using Zenject;
using Signals.Building;
using Signals.Resource;
using Signals.UI;
using Util;

namespace Game
{
    public class Main : MonoInstaller
    {
        public override void InstallBindings()
        {

            SignalBusInstaller.Install(Container);
            
            Container.Bind<Configuration>().AsSingle().NonLazy();
            Container.Bind<BuildingManager>().AsSingle().NonLazy();
            Container.Bind<UiSignals>().AsSingle().NonLazy();
            Container.Bind<BuildingSignals>().AsSingle().NonLazy();
            Container.Bind<ResourceSignals>().AsSingle().NonLazy();
            
            Container.DeclareSignal<BuildingButtonClickedSignal>();
            Container.DeclareSignal<BuildingPlacedSignal>();
        }

    }
    

}
