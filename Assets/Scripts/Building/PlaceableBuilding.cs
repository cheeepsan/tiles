using System;
using System.Collections;
using UnityEngine;
using Util;

namespace BuildingNS
{
    public class PlaceableBuilding : MonoBehaviour // to interface
    {
        public bool canBuild;
        private int _builtPercentage = 0;
        private CfgBuilding _buildingConfig;
        private bool _isBuilt = false;
        
        private void Start()
        {
            canBuild = true;
            Debug.Log("Building created");
        }
        
        public void Place()
        {
            IEnumerator coroutine = BuildingProcess();
            StartCoroutine(coroutine);
        }

        public void SetBuildingConfig(CfgBuilding buildingCfg)
        {
            _buildingConfig = buildingCfg;
        }
        
        public void OnGUI()
        {

            Vector2 targetPos;
            targetPos = Camera.main.WorldToScreenPoint(transform.position);

            float yOffset = 60; // not fixed? 
            float xOffset = 30;

            GUI.Box(new Rect(targetPos.x - xOffset, Screen.height - targetPos.y - yOffset, 60, 20), _builtPercentage + "/" + 100);
        }

        private IEnumerator BuildingProcess()
        {
            while (_builtPercentage != 100)
            {
                _builtPercentage += 1;
                yield return new WaitForSeconds(0.01f);
            }

            _isBuilt = true;
            SpawnWorker();
        }

        private void SpawnWorker()
        {
            if (_buildingConfig.unit != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    var rGameObject = (GameObject)Resources.Load(_buildingConfig.unit.path);
                    Instantiate(rGameObject, this.transform);
                }
  
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != 6)
            {
                canBuild = false;
                Debug.Log("An object entered. " + other.name);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer != 6)
            {
                if (canBuild) canBuild = false;
                
                Debug.Log("An object is still inside of the trigger " + other.name);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer != 6)
            {
                canBuild = true;
                Debug.Log("An object left.");
            }
        }
    }
}