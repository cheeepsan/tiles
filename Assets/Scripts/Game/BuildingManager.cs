using System;
using System.Collections.Generic;
using BuildingNS;
using Game.BuildingNS;
using Zenject;

namespace Game
{
    // TODO: Init building manager on start
    public class BuildingManager : IInitializable
    {
        [Inject] private readonly BuildingSignals _buildingSignals;

        private Dictionary<string, Building> _placedBuildings;
        
        public BuildingManager()
        {
            _placedBuildings = new Dictionary<string, Building>();
            
        }

        public string GetStateInfo()
        {
            return $"Building amount: {_placedBuildings.Count} \n";
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

        public void Initialize()
        {
            SubscribeOnBuildingPlacedEvent();
        }
    }
}