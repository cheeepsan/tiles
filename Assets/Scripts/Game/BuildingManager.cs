using System;
using System.Collections.Generic;
using BuildingNS;
using Game.BuildingNS;

namespace Game
{
    public class BuildingManager 
    {
        private readonly BuildingSignals _buildingSignals;

        private Dictionary<string, Building> _placedBuildings;
        
        public BuildingManager(BuildingSignals buildingSignals)
        {
            _placedBuildings = new Dictionary<string, Building>();
            _buildingSignals = buildingSignals;
            SubscribeOnBuildingPlacedEvent();
        }

        public string GetStateInfo()
        {
            var buildingsString = $"Building amount: {_placedBuildings.Count} \n";
            
            foreach (var keyValue in _placedBuildings)
            {
                buildingsString += $"{keyValue.Key} : {keyValue.Value.GetBuildingConfiguration().ToString()} \n";
            }

            return buildingsString;
        }

        private void SubscribeOnBuildingPlacedEvent()
        {
            _buildingSignals.Subscribe2<BuildingPlacedSignal, Dictionary<string, Building>>( (x, b) =>
                BuildingPlaced(x, b)
                , _placedBuildings);
        }
        
        private Action<BuildingPlacedSignal, Dictionary<string, Building>> BuildingPlaced = (s, d) =>
        {
            string uuid = Guid.NewGuid().ToString();
            Building building = new Building(s.buildingConfig);
            d.Add(uuid, building);
        };
    }
}