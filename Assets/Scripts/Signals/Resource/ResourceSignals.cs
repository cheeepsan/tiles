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

        public void FireAskForAvailableResource(AskForAvailableResourceSignal s)
        {
            _resourceSignalBus.Fire(s);
        }

        public void Subscribe2<IReseourceSignal, T>(Action<IReseourceSignal, T> actionOnFire, T a)
        {
            _resourceSignalBus.Subscribe(typeof(IReseourceSignal), (obj) => { actionOnFire((IReseourceSignal)obj, a); });
        }
    }
}