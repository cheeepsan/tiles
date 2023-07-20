using System;

using System.Collections.Generic;
using System.Linq;

using BuildingNS;
using Common;
using Constant;
using GridNS;
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
        [Inject] private Configuration _configuration;
        [Inject] private FarmPlotFactory _farmPlotFactory;
        [Inject] private GroundTileset _groundTileset;

        private readonly ResourceSignals _resourceSignals;

        private Dictionary<String, Resource> _resources;
        private Dictionary<String, PlaceableBuilding> _buildings;
        private Dictionary<String, String> _assignedResourceToBuilding;
        private Dictionary<String, String> _assignedBuildingToResource; // TODO ???

        private Dictionary<String, Transform> _farmPlots;
        
        private GameObject _hierarchyParent;
        public ResourceManager(ResourceSignals resourceSignals)
        {
            _hierarchyParent = GameObject.Find("Objects"); // hack, move to own static class
            _resourceSignals = resourceSignals;
            _resources = new Dictionary<string, Resource>();
            _buildings = new Dictionary<string, PlaceableBuilding>();
            _assignedResourceToBuilding = new Dictionary<string, string>();
            _assignedBuildingToResource = new Dictionary<string, string>();
            
            _farmPlots = new Dictionary<String, Transform>();
            TimeManager.On10Tick += delegate(object sender, TimeManager.On10TickEventArgs args)
            {
                PingAvailableResources();
            };
            
            TimeManager.OnMonthChange += delegate(object sender, TimeManager.OnMonthChangeEventArgs args)
            {

                YearlyCycle(args);
            };

            SubscribeToSignals();
        }


        private void YearlyCycle(TimeManager.OnMonthChangeEventArgs args)
        {
            /*
             * 0 - 3 flood
             * 4 - 7 field works
             * 8 - 11 gather
             */
            
            List<int> floodMonth = new List<int>() { 0, 1, 2 };

            int month = args.month;
            if (floodMonth.Contains(month))
            {
                // flood
                if (month == 0)
                {
                }
            }

            if (month == 4)
            {
                // remove flood
                // create farms
                CreateFarms();
            }
        }

        private void CreateFarms()
        {
            // TODO clean up. Farms is id 
            CfgBuilding farm = _configuration.GetCfgWorldObjectsList().Find(x => x.id == ResourceConstants.FARM_ID);
            GameObject fromResources = (GameObject)Resources.Load(farm.path);

            int totalFarms = _buildings.Values.Count(x => x.preferredResource == ResourceType.Farm);
            
            foreach (var key in _farmPlots.Take(totalFarms * 2))
            {
                var t = key.Value.transform;

                var farmPlot = _farmPlotFactory.Create(fromResources,  _hierarchyParent.transform);
                farmPlot.transform.position = new Vector3(t.position.x, t.position.y + 0.5f, t.position.z);

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
                // todo, resolve buildings and resources in batch 
                foreach (PlaceableBuilding building in availableBuildings)
                {
                    if (availableResources.Count > 0)
                    {
                        var preferredResource = building.preferredResource;
                        Resource foundPreferredResource = availableResources.Find(x => x.GetResourceType() == preferredResource);
                        if (foundPreferredResource != null)
                        {
                            FreeCurrentResourceOfBuilding(building); // free prev resource for next cycle
                            
                            building.SetCurrentResource(foundPreferredResource);
                            foundPreferredResource.SetAvailable(false);
                            availableResources.Remove(foundPreferredResource);
                            
                            
                            _assignedResourceToBuilding[foundPreferredResource.GetId()] = building.GetId();
                            _assignedBuildingToResource[building.GetId()] = foundPreferredResource.GetId();
                        }
                        else
                        {
                            ResolveResourceForBuilding(building, availableResources);
                        } 
                    }
                }
            }

            availableResources.Clear();
            availableBuildings.Clear();
        }
        
        private void ResolveResourceForBuilding(PlaceableBuilding building, List<Resource> availableResources)
        {
            var buildingType = building.GetBuildingType();
            var filteredResources = buildingType switch
            {
                "farm" => availableResources.FindAll(x => new List<ResourceType>(){ResourceType.Fruits}.Contains(x.GetResourceType())),
                "small_farm" => availableResources.FindAll(x => new List<ResourceType>(){ResourceType.Fruits}.Contains(x.GetResourceType())),
                _ => new List<Resource>()
            };

            if (filteredResources.Count > 0)
            {
                FreeCurrentResourceOfBuilding(building); // free prev resource for next cycle
                
                var resourceToGive = filteredResources.First();
                building.SetCurrentResource(resourceToGive);
                resourceToGive.SetAvailable(false);
                availableResources.Remove(resourceToGive);
                _assignedResourceToBuilding[resourceToGive.GetId()] = building.GetId();
                _assignedBuildingToResource[building.GetId()] = resourceToGive.GetId();
            }
        }

        private void FreeCurrentResourceOfBuilding(PlaceableBuilding building)
        {
            var currentResource = building.GetCurrentResource();
            if (currentResource != null)
            {
                currentResource.SetAvailable(true);
            }
        }

        private void AddFarmPlot(Transform farmPlotCoord)
        {
            String farmPlotGuid = _groundTileset.GetGroundTileByPosition(farmPlotCoord.position);
            
            if (!_farmPlots.ContainsKey(farmPlotGuid))
            {
                _farmPlots.Add(farmPlotGuid, farmPlotCoord);
            }
        }

        private void SubscribeToSignals()
        {
            SubscribeToResourceAvailableSignal();
            SubscribeToBuildingRegistered();
            SubscribeToAddAvailableFarmPlot();
            SubscribeToResourceDepleted();
        }

        private void SubscribeToResourceDepleted()
        {
            _resourceSignals.Subscribe<ResourceDepleted>
                ((r) =>
                {
                    var reseource = r.depletedResource;
                    var resourceUuid = reseource.GetId();
                    this._resources.Remove(resourceUuid);
                    var buildingUuid = _assignedResourceToBuilding[resourceUuid];
                    this._buildings[buildingUuid].SetAvailable(true);

                    GameObject.Destroy(reseource.gameObject);
                });
        }
        
        private void SubscribeToBuildingRegistered()
        {
            _resourceSignals.Subscribe<RegisterBuildingSignal>
                ((x) => { _buildings.Add(x.sender.GetId(), x.sender); });
        }


        private void SubscribeToAddAvailableFarmPlot()
        {
            _resourceSignals.Subscribe<AddAvailableFarmPlotSignal>
                ((x) => { AddFarmPlot(x.farmPlotTransform); });
        }

        private void SubscribeToResourceAvailableSignal()
        {
            Action<ResourceAvailableSignal, Dictionary<String, Resource>> ResourceAvailable = (s, d) =>
            {
                d.Add(s.resourceId, s.resource);
            };

            _resourceSignals.Subscribe2<ResourceAvailableSignal, Dictionary<String, Resource>>((x, b) =>
                    ResourceAvailable(x, b)
                , _resources);
        }
    }
}