using System;
using System.Collections.Generic;
using BuildingNS;
using Common.Signals;
using ResourceNS.Enum;
using Signals.ResourceNS;
using Signals.StockpileNS;
using Signals.UI;
using UnitNS;
using Zenject;

namespace Game
{
    public class StockpileManager
    {
        [Inject] private readonly UiSignals _uiSignals;
        
        private readonly StockpileSignals _stockpileSignals;
        
        private Dictionary<ResourceType, float> _accumulatedResources;
        private Queue<ResourceOperationQueueSignalStruct> _storingQueue;
        private Queue<ResourceOperationQueueSignalStruct> _retrieveQueue;

        public StockpileManager(StockpileSignals stockpileSignals)
        {
            _stockpileSignals = stockpileSignals;
            _accumulatedResources = new Dictionary<ResourceType, float>();
            _storingQueue = new Queue<ResourceOperationQueueSignalStruct>(); 
            _retrieveQueue = new Queue<ResourceOperationQueueSignalStruct>(); 
            
            TimeManager.On10Tick += delegate(object sender, TimeManager.On10TickEventArgs args)
            {
                DequeueResourceQueue();
            };
            
            SubscribeToAddResourceToQueue();
            SubscribeToRetrieveResourceQueue();
        }
        
        private void AddResourceToQueue(Tuple<ResourceType, float> r,  PlaceableBuilding b, Unit u)
        {
            _storingQueue.Enqueue(new ResourceOperationQueueSignalStruct(r, b, u));
        }
        
        private void AddRequestToRetrieveQueue(Tuple<ResourceType, float> r, PlaceableBuilding b)
        {
            _retrieveQueue.Enqueue(new ResourceOperationQueueSignalStruct(r, b));
        }

        // should below computation be blocked so no overlaps would occur?
        private void DequeueResourceQueue()
        {
            bool dataInQueue = _storingQueue.Count > 0;
            if (dataInQueue)
            {
                while (_storingQueue.Count > 0)
                {
                    ResourceOperationQueueSignalStruct element = _storingQueue.Dequeue();
                    
                    PlaceableBuilding building = element.GetBuilding();
                    Tuple<ResourceType, float> resourceData = element.GetResource();
                    
                    if (_accumulatedResources.ContainsKey(resourceData.Item1))
                    {
                        if (_accumulatedResources.TryGetValue(resourceData.Item1, out float resourceValue))
                        {
                            _accumulatedResources[resourceData.Item1] = resourceValue + resourceData.Item2;
                        }
                    }
                    else
                    {
                        _accumulatedResources.Add(resourceData.Item1, resourceData.Item2);
                    }
                    building.ResourceStoredToStockpile(element.GetUnit());
                }

                _uiSignals.FireUpdateResourcesViewSignal(new UpdateResourcesViewSignal()
                    { resources = _accumulatedResources });
            }

            bool dataInRetrieveQueue = _retrieveQueue.Count > 0;
            if (dataInRetrieveQueue)
            {
                while (_retrieveQueue.Count > 0)
                {
                    ResourceOperationQueueSignalStruct element = _retrieveQueue.Dequeue();
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
                ((x) => { AddResourceToQueue(x.resource, x.building, x.unit); });
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