using Zenject;
using UnityEngine;
using System.Collections;
using Game.BuildingNS;
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
            
            Container.Bind<UiSignals>().AsSingle().NonLazy();
            Container.Bind<BuildingSignals>().AsSingle().NonLazy();
    


            Container.DeclareSignal<BuildingButtonClickedSignal>();
            Container.DeclareSignal<BuildingPlacedSignal>();
   
            //BindSignalBuses();
            //DeclareSignals();
   
          
        }

    }
    

}
