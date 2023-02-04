using System;
using System.Collections.Generic;
using BuildingNS;
using Signals.Building;

namespace Game
{
    public class BuildingManager 
    {
        private readonly BuildingSignals _buildingSignals;

        private Dictionary<string, PlaceableBuilding> _placedBuildings;
        
        public BuildingManager(BuildingSignals buildingSignals)
        {
            _placedBuildings = new Dictionary<string, PlaceableBuilding>();
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
        
        private void SubscribeOnBuildingPlacedEvent()
        {
            _buildingSignals.Subscribe2<BuildingPlacedSignal, Dictionary<string, PlaceableBuilding>>( (x, b) =>
                BuildingPlaced(x, b)
                , _placedBuildings);
        }
        
        private Action<BuildingPlacedSignal, Dictionary<string, PlaceableBuilding>> BuildingPlaced = (s, d) =>
        {
            d.Add(s.placeableBuilding.GetId(), s.placeableBuilding);
        };
    }
}