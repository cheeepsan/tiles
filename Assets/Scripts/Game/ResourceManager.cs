using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingNS;
using ResourceNS;
using ResourceNS.Enum;
using Signals.ResourceNS;
using Signals.UI;
using UnityEngine;
using Util;
using Zenject;


namespace Game
{
    /// <summary>
    ///  Keep track of available resource spots: fruits, available farms etc...
    /// </summary>
    public class ResourceManager : IDisposable
    {
        [Inject] private readonly UiSignals _uiSignals;

        private readonly ResourceSignals _resourceSignals;

        private Dictionary<string, Resource> _resources;
        private Dictionary<string, PlaceableBuilding> _buildings;

        private Dictionary<ResourceType, float> _accumulatedResources;
        private Queue<Tuple<ResourceType, float>> _storingQueue;

        public ResourceManager(ResourceSignals resourceSignals)
        {
            _resourceSignals = resourceSignals;
            _resources = new Dictionary<string, Resource>();
            _buildings = new Dictionary<string, PlaceableBuilding>();
            _accumulatedResources = new Dictionary<ResourceType, float>();
            _storingQueue = new Queue<Tuple<ResourceType, float>>(); // TODO: Is it even needed. Is it even smart

            TimeManager.On10Tick += delegate(object sender, TimeManager.On10TickEventArgs args)
            {
                PingAvailableResources();
                DequeueResourceQueue();
            };

            SubscribeToSignals();
        }

        public void AddResourceToQueue(Tuple<ResourceType, float> r)
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
                    if (_accumulatedResources.TryGetValue(element.Item1, out float resourceValue))
                    {
                        _accumulatedResources[element.Item1] = resourceValue + element.Item2;
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

        private void PingAvailableResources()
        {
            //Debug.Log("Polling data for resource manager");
            List<Resource> availableResources = new List<Resource>();
            List<PlaceableBuilding> availableBuildings = new List<PlaceableBuilding>();

            foreach (var b in _buildings.Values)
            {
                if (b.IsAvailable())
                {
                    availableBuildings.Add(b);
                }
            }

            foreach (var r in _resources.Values)
            {
                if (r.IsAvailable())
                {
                    availableResources.Add(r);
                }
            }

            if (availableResources.Count > 0 && availableBuildings.Count > 0)
            {
                // todo, sort building by priorities
                // todo, get available resources by preferred type
                // todo, resolve buildings and resources in batch 

                PlaceableBuilding building = availableBuildings.First();
                Resource resource = availableResources.First();
                building.SetCurrentResource(resource);
            }

            availableResources.Clear();
            availableBuildings.Clear();
        }


        private void SubscribeToSignals()
        {
            SubscribeToResourceAvailableSignal();
            SubscribeToBuildingRegistered();
            SubscribeToAddResourceToQueue();
        }

        private void SubscribeToBuildingRegistered()
        {
            _resourceSignals.Subscribe<RegisterBuildingSignal>
                ((x) => { _buildings.Add(x.sender.GetId(), x.sender); });
        }

        private void SubscribeToAddResourceToQueue()
        {
            _resourceSignals.Subscribe<AddResourceToQueueSignal>
                ((x) => { AddResourceToQueue(x.resource); });
        }

        private void SubscribeToResourceAvailableSignal()
        {
            Action<ResourceAvailableSignal, Dictionary<string, Resource>> ResourceAvailable = (s, d) =>
            {
                d.Add(s.resourceId, s.resource);
            };

            _resourceSignals.Subscribe2<ResourceAvailableSignal, Dictionary<string, Resource>>((x, b) =>
                    ResourceAvailable(x, b)
                , _resources);
        }

        public void Dispose()
        {
            //_timer.Dispose();
            Debug.Log("CLOSING RESOURCE MANAGER");
        }

        /*
         TIMER WORKS ON ANOTHER THREAD, AS A QUICK HACK COROUTINE MONOBEHAVIOUR WAS USED
         
        private void RunPollingTask()
        {
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(5);

            _timer = new System.Threading.Timer((e) =>
            {
                PingAvailableResources();   
            }, null, startTimeSpan, periodTimeSpan);
            
        }
         
         /// <summary>
         ///  Polling from another thread raises problems within UnityObjects: UnityEngine.UnityException:
         /// get_gameObject can only be called from the main thread.
         ///
         /// Objects have to be stored without unity Monobehaviour data ?
         /// </summary>
         private void PingAvailableResources()
         {
             Debug.Log("Polling data for resource manager");
             List<Resource> availableResources = new List<Resource>();
             List<PlaceableBuilding> availableBuildings = new List<PlaceableBuilding>();

             foreach (var b in _buildings.Values)
             {
                 if (b.IsAvailable())
                 {
                     availableBuildings.Add(b);
                 }
             }

             foreach (var r in _resources.Values)
             {
                 if (r.IsAvailable())
                 {
                     availableResources.Add(r);
                 }
             }

             if (availableResources.Count > 0 && availableBuildings.Count > 0)
             {
                 // todo, sort building by priorities
                 // todo, get available resources by preferred type
                 // todo, resolve buildings and resources in batch 

                 PlaceableBuilding building = availableBuildings.First();
                 Resource resource = availableResources.First();
                 building.SetCurrentResource(resource);
             }

             availableResources.Clear();
             availableBuildings.Clear();
             //yield return new WaitForSeconds(5f);
         }
         */
    }
}