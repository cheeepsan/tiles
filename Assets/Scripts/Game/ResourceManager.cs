using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingNS;
using ResourceNS;
using Signals.ResourceNS;
using UnityEngine;
using Util;


namespace Game
{
    /// <summary>
    ///  Keep track of available resource spots: fruits, available farms etc...
    /// </summary>
    public class ResourceManager : IDisposable
    {
        private readonly ResourceSignals _resourceSignals;

        private Dictionary<string, Resource> _resources;
        private Dictionary<string, PlaceableBuilding> _buildings;

        //private System.Threading.Timer _timer;

        private GameObject _coroutineRunnerGameObject;
        private CoroutineRunner _coroutineRunner;
        
        public ResourceManager(ResourceSignals resourceSignals)
        {
            _resourceSignals = resourceSignals;
            _resources = new Dictionary<string, Resource>();
            _buildings = new Dictionary<string, PlaceableBuilding>();

            _coroutineRunnerGameObject = new GameObject();
            _coroutineRunner = _coroutineRunnerGameObject.AddComponent<CoroutineRunner>();
            _coroutineRunner.SetCoroutine(PingAvailableResources());
            _coroutineRunner.RunCoroutine();

            SubscribeToSignals();
        }

        private IEnumerator PingAvailableResources()
        {
            while (true) // nice
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
                yield return new WaitForSeconds(5f);
            }
        }
        

        private void SubscribeToSignals()
        {
            SubscribeToResourceAvailableSignal();
            SubscribeToBuildingRegistered();
        }

        private void SubscribeToBuildingRegistered()
        {
            _resourceSignals.Subscribe<RegisterBuildingSignal>
                ((x) => { _buildings.Add(x.sender.GetId(), x.sender); });
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