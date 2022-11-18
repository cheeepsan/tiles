using BuildingNS;
using Zenject;
using Signals.Building;
using Signals.ResourceNS;
using Signals.UI;
using UnitNS;
using UnityEngine;
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
            Container.Bind<ResourceManager>().AsSingle().NonLazy();
            
            Container.Bind<UiSignals>().AsSingle().NonLazy();
            Container.Bind<BuildingSignals>().AsSingle().NonLazy();
            Container.Bind<ResourceSignals>().AsSingle().NonLazy();
            
            Container.DeclareSignal<BuildingButtonClickedSignal>();
            Container.DeclareSignal<BuildingPlacedSignal>();

            Container.DeclareSignal<ResourceAvailableSignal>();
            Container.DeclareSignal<AskForAvailableResourceSignal>();

            Container.BindFactory<Object, PlaceableBuilding, PlaceableBuildingFactory>()
                .FromFactory<PrefabFactory<PlaceableBuilding>>();
            
            Container.BindFactory<Object, Unit, UnitFactory>().FromFactory<PrefabFactory<Unit>>();
        }

    }
    

}
