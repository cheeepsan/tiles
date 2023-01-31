using System;
using UnityEngine;
using Zenject;

namespace Signals.ResourceNS
{
    public class ResourceSignals : IInitializable
    {
        private readonly SignalBus _resourceSignalBus;

        public ResourceSignals(SignalBus resourceSignalBus)
        {
            _resourceSignalBus = resourceSignalBus;
        }
        
        public void Initialize()
        {
            Debug.Log("Resource signals init");
        }

        public void FireResourceAvailableSignal(ResourceAvailableSignal a)
        {
            _resourceSignalBus.Fire(a);
        }

        public void FireResourceIsSet(ResourceIsSetToBuildingSignal s)
        {
            _resourceSignalBus.Fire(s);
        }
        
        public void FireRegisterBuilding(RegisterBuildingSignal s)
        {
            _resourceSignalBus.Fire(s);
        }
        
        public void FireAddResourceToQueue(AddResourceToQueueSignal s)
        {
            _resourceSignalBus.Fire(s);
        }

        
        public void Subscribe<IResourceSignal>(Action<IResourceSignal> actionOnFire)
        {
            _resourceSignalBus.Subscribe(actionOnFire);
        }
        
        public void Subscribe2<IResourceSignal, T>(Action<IResourceSignal, T> actionOnFire, T a)
        {
            _resourceSignalBus.Subscribe(typeof(IResourceSignal), (obj) => { actionOnFire((IResourceSignal)obj, a); });
        }
    }
}