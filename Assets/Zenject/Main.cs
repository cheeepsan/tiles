using System;
using BuildingNS;
using Zenject;
using Signals.Building;
using Signals.ResourceNS;
using Signals.UI;
using UnitNS;
using Util;
using Object = UnityEngine.Object;

namespace Game
{
    public class Main : MonoInstaller
    {
        public override void InstallBindings()
        {

            SignalBusInstaller.Install(Container);
            
            Container.Bind<Configuration>().AsSingle().NonLazy();
            
            Container.Bind<BuildingManager>().AsSingle().NonLazy();
            Container.Bind<IDisposable>().To<ResourceManager>().AsSingle().NonLazy();
            
            Container.Bind<UiSignals>().AsSingle().NonLazy();
            Container.Bind<BuildingSignals>().AsSingle().NonLazy();
            Container.Bind<ResourceSignals>().AsSingle().NonLazy();
            
            Container.DeclareSignal<BuildingButtonClickedSignal>();
            Container.DeclareSignal<BuildingPlacedSignal>();

            Container.DeclareSignal<ResourceAvailableSignal>();
            Container.DeclareSignal<RegisterBuildingSignal>();
            Container.DeclareSignal<ResourceIsSetToBuildingSignal>();
            Container.DeclareSignal<AddResourceToQueueSignal>();
            Container.DeclareSignal<UpdateResourcesViewSignal>();
            
            Container.BindFactory<Object, PlaceableBuilding, PlaceableBuildingFactory>()
                .FromFactory<PrefabFactory<PlaceableBuilding>>();
            Container.BindFactory<Object, BuildingElement, BuildingElementFactory>()
                .FromFactory<PrefabFactory<BuildingElement>>();
            
            Container.BindFactory<Object, Unit, UnitFactory>().FromFactory<PrefabFactory<Unit>>();

            Container.Bind<TimeManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        }

    }
    

}
