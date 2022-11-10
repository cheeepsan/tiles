using UnityEngine;
using Zenject;

namespace Signals.Resource
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
    }
}