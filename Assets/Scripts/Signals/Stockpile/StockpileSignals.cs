using System;
using Signals.ResourceNS;
using UnityEngine;
using Zenject;

namespace Signals.StockpileNS
{
    public class StockpileSignals : IInitializable
    {
        private readonly SignalBus _stockpileSignalBus;

        public StockpileSignals(SignalBus stockpileSignalBus)
        {
            _stockpileSignalBus = stockpileSignalBus;
        }

        public void Initialize()
        {
            Debug.Log("Stockpile signals init");
        }
        
        public void FireAddResourceToQueue(AddResourceToQueueSignal s)
        {
            _stockpileSignalBus.Fire(s);
        }
        
        public void FireAddResourceRetrieveToQueue(RetrieveResourceQueueSignal s)
        {
            _stockpileSignalBus.Fire(s);
        }

        public void FireResourceAddedToQueueResponse(ResourceAddedToQueueResponseSignal r)
        {
            _stockpileSignalBus.Fire(r);
        }
        
        public void Subscribe<IResourceSignal>(Action<IResourceSignal> actionOnFire)
        {
            _stockpileSignalBus.Subscribe(actionOnFire);
        }
        
        public void Subscribe2<IResourceSignal, T>(Action<IResourceSignal, T> actionOnFire, T a)
        {
            _stockpileSignalBus.Subscribe(typeof(IResourceSignal), (obj) => { actionOnFire((IResourceSignal)obj, a); });
        }
    }
}