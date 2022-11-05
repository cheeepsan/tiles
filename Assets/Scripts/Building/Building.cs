using UnityEngine;
using Util;

namespace BuildingNS
{
    public class Building
    {
        private GameObject _buildingObject; // can be nullable?
        private CfgBuilding _buildingConfig;
        
        public Building(GameObject gameObject, CfgBuilding buildingConfig)
        {
            _buildingObject = gameObject;
            _buildingConfig = buildingConfig;
        }
        
        public Building(CfgBuilding buildingConfig)
        {
            _buildingConfig = buildingConfig;
        }

        public void SetGameObject(GameObject gameObject)
        {
            _buildingObject = gameObject;
        }

        public GameObject GetGameObject()
        {
            return _buildingObject;
        }

        public void SetBuildingConfiguration(CfgBuilding cfg)
        {
            _buildingConfig = cfg;
        }

        public CfgBuilding GetBuildingConfiguration()
        {
            return _buildingConfig;
        }
    }
}