using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildingNS;
using Game;
using Newtonsoft.Json;
using UnityEngine;
using Util;
using Zenject;

namespace SaveStateNS
{
    public class SaveState
    {
        [Inject] private readonly BuildingManager _buildingManager;
        [Inject] private readonly ResourceManager _resourceManager;
        [Inject] private readonly TimeManager _timeManager;

        public SaveState()
        {
            
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

            SaveStateMasterObject saveStateMasterObject = new SaveStateMasterObject(buildingsToSave, _timeManager.GetCurrentTick());

            String a = JsonConvert.SerializeObject(saveStateMasterObject);
            File.WriteAllText("save.json", a);
        }

        public void LoadStateFromJson()
        {
            Debug.Log("LOAD STATE");
        }
    }
}