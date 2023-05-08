using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildingNS;
using Game;
using Newtonsoft.Json;
using ResourceNS.Enum;
using Signals.Building;
using UnityEngine;
using Util;
using Zenject;

namespace SaveStateNS
{
    public class SaveState
    {
        [Inject] private readonly BuildingManager _buildingManager;
        [Inject] private readonly ResourceManager _resourceManager;
        [Inject] private readonly StockpileManager _stockpileManager;
        [Inject] private readonly TimeManager _timeManager;
        [Inject] private readonly PlaceableBuildingFactory _placeableBuildingFactory;
        [Inject] private readonly BuildingSignals _buildingSignals;

        private GameObject _buildingHierarchyParent;

        public SaveState()
        {
            _buildingHierarchyParent = GameObject.Find("Objects"); // hack, move to own static class
        }

        public void SaveStateToJson()
        {
            Debug.Log("SAVE STATE");
            List<PlaceableBuilding> buildingList =
                _buildingManager.GetAllBuildings().Select(x => x.Value).ToList();

            List<BuildingSaveState> buildingsToSave = new List<BuildingSaveState>();

            foreach (var building in buildingList)
            {
                BuildingSaveState buildingSaveState =
                    new BuildingSaveState(building.transform, building.GetBuildingConfig(), building.GetId());
                buildingsToSave.Add(buildingSaveState);
            }

            Dictionary<ResourceType, float> resources = _stockpileManager.GetAllResources();

            SaveStateMasterObject saveStateMasterObject =
                new SaveStateMasterObject(buildingsToSave, resources, _timeManager.GetCurrentTick());

            String a = JsonConvert.SerializeObject(saveStateMasterObject);
            File.WriteAllText("save.json", a);
        }

        /// <summary>
        /// Restores buildings and resources from json state.
        /// Reload of the existing state doesn't currently work. Needs a clean state now.
        /// TODO: fix reload
        /// </summary>
        public void LoadStateFromJson()
        {
            Debug.Log("LOAD STATE");
            var text = File.ReadAllText("save.json");

            SaveStateMasterObject saveState = JsonConvert.DeserializeObject<SaveStateMasterObject>(text);
            
            RestoreBuildings(saveState);
            RestoreResources(saveState);
            _timeManager.SetCurrentTick(saveState.timeTick);
        }


        private void RestoreResources(SaveStateMasterObject saveState)
        {
            Dictionary<ResourceType, float> resource = saveState.resources;
            _stockpileManager.SetAllResources(resource);
        }

        private void RestoreBuildings(SaveStateMasterObject saveState)
        {
            Dictionary<string, PlaceableBuilding> buildingDict = _buildingManager.GetAllBuildings();

            foreach (var keyValue in buildingDict)
            {
                // Should unsubscribe from everything. DISPOSABLE?
                GameObject.Destroy(keyValue.Value);
            }

            buildingDict.Clear();

            foreach (BuildingSaveState building in saveState.buildings)
            {
                GameObject fromResources = (GameObject)Resources.Load(building.config.path);
                Building toPlace = new Building(fromResources, building.config);
                PlaceableBuilding instantiatedBuilding =
                    _placeableBuildingFactory.Create(toPlace.GetGameObject(), _buildingHierarchyParent.transform);

                instantiatedBuilding.transform.position = new Vector3(building.x, building.y, building.z);
                instantiatedBuilding.SetBuildingConfig(building.config);
                instantiatedBuilding.SetId(building.id);
                instantiatedBuilding.SetIsLoaded(true);

                BuildingPlacedSignal buildingPlacedSignal = new BuildingPlacedSignal
                {
                    placedTimestamp = DateTime.Now,
                    placeableBuilding = instantiatedBuilding
                };
                _buildingSignals.FireBuildingPlacedEvent(buildingPlacedSignal);
            }
        }
    }
}