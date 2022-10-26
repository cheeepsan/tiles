using Zenject;
using UnityEngine;
using System.Collections;
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

            // https://github.com/modesttree/Zenject/blob/master/Documentation/Signals.md
            Container.DeclareSignal<BuildingButtonClickedSignal>();
        }
    }
    

}
