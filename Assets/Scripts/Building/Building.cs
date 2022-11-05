using UnityEngine;

namespace BuildingNS
{
    public class Building
    {
        private GameObject _buildingObject;

        public Building(GameObject gameObject)
        {
            _buildingObject = gameObject;
        }

        public void SetGameObject(GameObject gameObject)
        {
            _buildingObject = gameObject;
        }

        public GameObject GetGameObject()
        {
            return _buildingObject;
        }
    }
}