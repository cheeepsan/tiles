using System;
using BuildingNS;
using Common.Logic;
using GridNS;
using ResourceNS;
using SaveStateNS;
using Zenject;
using Signals.Building;
using Signals.ResourceNS;
using Signals.StockpileNS;
using Signals.UI;
using TideNS;
using Ui.Common;
using UnitNS;
using UnityEngine.UI;
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
            Container.Bind<ResourceManager>().AsSingle().NonLazy();
            Container.Bind<StockpileManager>().AsSingle().NonLazy();
            Container.Bind<OnClickHighlightLogic>().AsSingle().NonLazy();
            
            Container.Bind<IInitializable>().To<TideManager>().AsSingle().NonLazy();

            Container.Bind<SaveState>().AsSingle().NonLazy();

            Container.Bind<UiSignals>().AsSingle().NonLazy();
            Container.Bind<BuildingSignals>().AsSingle().NonLazy();
            Container.Bind<ResourceSignals>().AsSingle().NonLazy();
            Container.Bind<StockpileSignals>().AsSingle().NonLazy();

            Container.Bind<GroundTileset>().AsSingle().NonLazy();
            Container.Bind<WaterTileset>().AsSingle().NonLazy();

            Container.DeclareSignal<BuildingButtonClickedSignal>();
            Container.DeclareSignal<BuildingPlacedSignal>();

            Container.DeclareSignal<ResourceAvailableSignal>();
            Container.DeclareSignal<RegisterBuildingSignal>();
            Container.DeclareSignal<ResourceIsSetToBuildingSignal>();
            
            Container.DeclareSignal<AddResourceToQueueSignal>();
            Container.DeclareSignal<RetrieveResourceQueueSignal>();
            Container.DeclareSignal<AddAvailableFarmPlotSignal>();
            Container.DeclareSignal<ResourceAddedToQueueResponseSignal>();
            
            Container.DeclareSignal<UpdateResourcesViewSignal>();
            Container.DeclareSignal<ResourceDepleted>();
            
            Container.DeclareSignal<BuildingInfoViewSignal>();
            
            Container.BindFactory<Object, PlaceableBuilding, PlaceableBuildingFactory>()
                .FromFactory<PrefabFactory<PlaceableBuilding>>();
            Container.BindFactory<Object, BuildingElement, BuildingElementFactory>()
                .FromFactory<PrefabFactory<BuildingElement>>();
            Container.BindFactory<Object, FarmPlot, FarmPlotFactory>()
                .FromFactory<PrefabFactory<FarmPlot>>();
            Container.BindFactory<Object, TideMesh, TideMeshFactory>()
                .FromFactory<PrefabFactory<TideMesh>>();
            
            Container.BindFactory<Object, Unit, UnitFactory>().FromFactory<PrefabFactory<Unit>>();
            
            Container.BindFactory<Object, Button, UiBuildingButtonFactory>().FromFactory<PrefabFactory<Button>>();

            Container.Bind<TimeManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.Bind<HighlightRenderer>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        }

    }
    

}
