using System;
using System.Collections.Generic;
using ResourceNS.Enum;
using Signals.ResourceNS;
using Signals.StockpileNS;
using Signals.UI;
using Zenject;

namespace Game
{
    public class StockpileManager
    {
        [Inject] private readonly UiSignals _uiSignals;
        
        private readonly StockpileSignals _stockpileSignals;
        
        private Dictionary<ResourceType, float> _accumulatedResources;
        private Queue<Tuple<ResourceType, float>> _storingQueue;

        public StockpileManager(StockpileSignals stockpileSignals)
        {
            _stockpileSignals = stockpileSignals;
            _accumulatedResources = new Dictionary<ResourceType, float>();
            _storingQueue = new Queue<Tuple<ResourceType, float>>(); 
            
            TimeManager.On10Tick += delegate(object sender, TimeManager.On10TickEventArgs args)
            {
                DequeueResourceQueue();
            };
            
            SubscribeToAddResourceToQueue();
        }
        
        private void AddResourceToQueue(Tuple<ResourceType, float> r)
        {
            _storingQueue.Enqueue(r);
        }

        // TODO: Resource add/removal should be synced. Maybe removal prio 1, addition may be skipped?
        private void DequeueResourceQueue()
        {
            bool dataInQueue = _storingQueue.Count > 0;
            if (dataInQueue)
            {
                while (_storingQueue.Count > 0)
                {
                    Tuple<ResourceType, float> element = _storingQueue.Dequeue();
                    if (_accumulatedResources.ContainsKey(element.Item1))
                    {
                        if (_accumulatedResources.TryGetValue(element.Item1, out float resourceValue))
                        {
                            _accumulatedResources[element.Item1] = resourceValue + element.Item2;
                        }
                    }
                    else
                    {
                        _accumulatedResources.Add(element.Item1, element.Item2);
                    }
                }

                _uiSignals.FireUpdateResourcesViewSignal(new UpdateResourcesViewSignal()
                    { resources = _accumulatedResources });
            }
        }
        
        private void SubscribeToAddResourceToQueue()
        {
            _stockpileSignals.Subscribe<AddResourceToQueueSignal>
                ((x) => { AddResourceToQueue(x.resource); });
        }
        
        public Dictionary<ResourceType, float> GetAllResources()
        {
            return _accumulatedResources;
        }
        
        public void SetAllResources(Dictionary<ResourceType, float> resource)
        {
            _accumulatedResources = resource;
            _uiSignals.FireUpdateResourcesViewSignal(new UpdateResourcesViewSignal()
                { resources = _accumulatedResources });
        }
    }
}