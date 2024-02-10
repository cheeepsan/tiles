using System;
using System.Collections.Generic;
using BuildingNS;
using Common.Signals;
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
        private Queue<RetrieveResourceQueueSignalStruct> _retrieveQueue;

        public StockpileManager(StockpileSignals stockpileSignals)
        {
            _stockpileSignals = stockpileSignals;
            _accumulatedResources = new Dictionary<ResourceType, float>();
            _storingQueue = new Queue<Tuple<ResourceType, float>>(); 
            _retrieveQueue = new Queue<RetrieveResourceQueueSignalStruct>(); 
            
            TimeManager.On10Tick += delegate(object sender, TimeManager.On10TickEventArgs args)
            {
                DequeueResourceQueue();
            };
            
            SubscribeToAddResourceToQueue();
            SubscribeToRetrieveResourceQueue();
        }
        
        private void AddResourceToQueue(Tuple<ResourceType, float> r)
        {
            _storingQueue.Enqueue(r);
        }
        
        private void AddRequestToRetrieveQueue(Tuple<ResourceType, float> r, PlaceableBuilding b)
        {
            _retrieveQueue.Enqueue(new RetrieveResourceQueueSignalStruct(r, b));
        }

        // should below computation be blocked so no overlaps would occur?
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

            bool dataInRetrieveQueue = _retrieveQueue.Count > 0;
            if (dataInRetrieveQueue)
            {
                while (_retrieveQueue.Count > 0)
                {
                    RetrieveResourceQueueSignalStruct element = _retrieveQueue.Dequeue();
                    Tuple<ResourceType, float> resourceData = element.GetResource();
                    if (_accumulatedResources.ContainsKey(resourceData.Item1))
                    {
                        if (_accumulatedResources.TryGetValue(resourceData.Item1, out float resourceValue) && resourceValue >= resourceData.Item2)
                        {
                            _accumulatedResources[resourceData.Item1] = resourceValue - resourceData.Item2;
                            element.GetBuilding().AddReservedResourceAmount(resourceData.Item2);
                        }
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
        
        private void SubscribeToRetrieveResourceQueue()
        {
            _stockpileSignals.Subscribe<RetrieveResourceQueueSignal>
                ((x) => { AddRequestToRetrieveQueue(x.resource, x.building); });
        }

        
        public Dictionary<ResourceType, float> GetAllResources()
        {
            return _accumulatedResources;
        }
        
        // Prob should be a signal since it's about SET
        public void SetAllResources(Dictionary<ResourceType, float> resource)
        {
            _accumulatedResources = resource;
            _uiSignals.FireUpdateResourcesViewSignal(new UpdateResourcesViewSignal()
                { resources = _accumulatedResources });
        }

        public float GetResourceAmount(ResourceType resourceType)
        {
            bool hasValue = _accumulatedResources.TryGetValue(resourceType, out float amount);
            
            if (hasValue)
            {
                return amount;
            }
            else
            {
                return 0f;
            }
        }
    }
}