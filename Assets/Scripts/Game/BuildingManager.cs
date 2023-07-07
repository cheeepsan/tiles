using System;
using System.Collections.Generic;
using BuildingNS;
using Signals.Building;
using Signals.UI;
using Util;
using Zenject;

namespace Game
{
    public class BuildingManager
    {

        [Inject] private Configuration _config;
        
        private readonly BuildingSignals _buildingSignals;
        private readonly UiSignals _uiSignals;

        private Dictionary<string, PlaceableBuilding> _placedBuildings;
        private List<int> _builtUniqueBuildings;

        public BuildingManager(BuildingSignals buildingSignals)
        {
            _placedBuildings = new Dictionary<string, PlaceableBuilding>();
            _builtUniqueBuildings = new List<int>();
            _buildingSignals = buildingSignals;
            SubscribeOnBuildingPlacedEvent();
        }

        public string GetStateInfo()
        {
            var buildingsString = $"Building amount: {_placedBuildings.Count} \n";
            
            return buildingsString;
        }
        
        public Dictionary<string, PlaceableBuilding> GetAllBuildings()
        {
            return _placedBuildings;
        }

        public List<int> GetBuiltUniqueBuildingList()
        {
            return _builtUniqueBuildings;
        }
        
        private void SubscribeOnBuildingPlacedEvent()
        {
            _buildingSignals.Subscribe3<BuildingPlacedSignal, Dictionary<string, PlaceableBuilding>, List<int>>( (x, b, l) =>
                BuildingPlaced(x, b, l)
                , _placedBuildings, _builtUniqueBuildings);
        }
        
        private Action<BuildingPlacedSignal, Dictionary<string, PlaceableBuilding>, List<int>> BuildingPlaced = (s, d, l) =>
        {
            int configId = s.placeableBuilding.GetBuildingConfig().id;
            d.Add(s.placeableBuilding.GetId(), s.placeableBuilding);
            if (!l.Contains(configId))
            {
                l.Add(configId);
            }
        };
        
    }
}