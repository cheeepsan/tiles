using System;
using Signals.UI;
using UnityEngine;
using Zenject;

namespace Game
{
    public class UiSignals : IInitializable
    {
        readonly SignalBus _uiSignalBus;

        public UiSignals(SignalBus uiSignalBus)
        {
            _uiSignalBus = uiSignalBus;
        }
        
        public void Initialize()
        {
            Debug.Log("UiSignalBus init");
        }

        public void FireBuildingButtonEvent(BuildingButtonClickedSignal b)
        {
            _uiSignalBus.Fire(b);
        }

        public void Subscribe<T>(Action<T> actionOnFire)
        {
            _uiSignalBus.Subscribe<T>(actionOnFire);
        }
        
        public void Subscribe2<IButtonClicked, T >(Action<IButtonClicked, T> actionOnFire, T a)
        {
            _uiSignalBus.Subscribe(typeof(IButtonClicked), (obj) => { actionOnFire((IButtonClicked)obj, a); });
        }
        
        public void Subscribe3<IButtonClicked, T, TA>(Action<IButtonClicked, T, TA> actionOnFire, T a, TA a1)
        {
            _uiSignalBus.Subscribe(typeof(IButtonClicked), (obj) => { actionOnFire((IButtonClicked)obj, a, a1); });
        }
    }
}