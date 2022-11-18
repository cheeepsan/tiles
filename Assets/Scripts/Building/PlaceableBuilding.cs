using System;
using System.Collections;
using System.Collections.Generic;
using UnitNS;
using UnityEngine;
using Util;
using Zenject;

namespace BuildingNS
{
    public class PlaceableBuilding : MonoBehaviour // to interface
    {
        [Inject] private readonly UnitFactory _unitFactory; 
        
        public bool canBuild;
        private int _builtPercentage = 0;
        private CfgBuilding _buildingConfig;

        private List<Unit> _workers; // todo change to array
        
        private void Start()
        {
            canBuild = true;
            _workers = new List<Unit>();
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
            
            SpawnWorker();
        }

        private void SpawnWorker()
        {
            if (_buildingConfig.unit != null)
            {
                GameObject unitGb = (GameObject)Resources.Load(_buildingConfig.unit.path);
                Unit unit = _unitFactory.Create(unitGb, this.transform.parent.transform);

                Vector3 parentPosition = this.transform.position;
                
                unit.transform.position.Set(parentPosition.x, parentPosition.y, parentPosition.z );
                unit.SetParentBuilding(this);
                _workers.Add(unit);
                    
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
                
                //Debug.Log("An object is still inside of the trigger " + other.name);
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