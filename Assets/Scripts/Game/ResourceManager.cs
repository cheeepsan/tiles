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
    public class ResourceManager
    {
        [Inject] private readonly UiSignals _uiSignals;

        private readonly ResourceSignals _resourceSignals;

        private Dictionary<string, Resource> _resources;
        private Dictionary<string, PlaceableBuilding> _buildings;

        public ResourceManager(ResourceSignals resourceSignals)
        {
            _resourceSignals = resourceSignals;
            _resources = new Dictionary<string, Resource>();
            _buildings = new Dictionary<string, PlaceableBuilding>();
            
            TimeManager.On10Tick += delegate(object sender, TimeManager.On10TickEventArgs args)
            {
                PingAvailableResources();
            };

            SubscribeToSignals();
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
    }
}