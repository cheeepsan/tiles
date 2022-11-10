using System;
using BuildingNS;
using UnityEngine;
using Zenject;

namespace Signals.Building
{
    public class BuildingSignals : IInitializable
    {
        private readonly SignalBus _buildingSignalBus;

        public BuildingSignals(SignalBus buildingSignalBus)
        {
            _buildingSignalBus = buildingSignalBus;
        }
        
        public void Initialize()
        {
            Debug.Log("BuildingSignals init");
        }

        public void FireBuildingPlacedEvent(BuildingPlacedSignal b)
        {
            _buildingSignalBus.Fire(b);
        }
        
        public void Subscribe<T>(Action<T> actionOnFire)
        {
            _buildingSignalBus.Subscribe<T>(actionOnFire);
        }
        
        public void Subscribe<T>(Action actionOnFire)
        {
            _buildingSignalBus.Subscribe<T>(actionOnFire);
        }

        public void Subscribe2<IBuildingSignal, T>(Action<IBuildingSignal, T> actionOnFire, T a)
        {
            _buildingSignalBus.Subscribe(typeof(IBuildingSignal), (obj) => { actionOnFire((IBuildingSignal)obj, a); });
        }
    }
}