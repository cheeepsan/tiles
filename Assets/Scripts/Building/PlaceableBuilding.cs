using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using ResourceNS;
using ResourceNS.Enum;
using Signals.ResourceNS;
using UnitNS;
using UnityEngine;
using Util;
using Zenject;
namespace BuildingNS
{
    public class PlaceableBuilding : MonoBehaviour
    {
        [Inject] private readonly UnitFactory _unitFactory;
        [Inject] private readonly ResourceSignals _resourceSignals;
        private int _builtPercentage = 0;
        private CfgBuilding _buildingConfig;
        private bool _isAvailable;
        private bool _isBuilding;    
        
        private Resource _currentResource;
        private List<Unit> _workers; // todo change to array

        private string _id;
            
        public bool canBuild;
 
        public ResourceType preferredResource;
        

        private void Start()
        {
            _id = Guid.NewGuid().ToString();
            canBuild = true;
            _workers = new List<Unit>();

        }
        
        public void Place()
        {
            _isBuilding = true;
            TimeManager.OnTick += delegate(object sender, TimeManager.OnTickEventArgs args)
            {
                BuildingOnTick();
            };
            
            // coroutine as alternate method
            //IEnumerator coroutine = BuildingProcess();
            //StartCoroutine(coroutine);
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
            if (_isBuilding)
            {
                GUI.Box(new Rect(targetPos.x - xOffset, Screen.height - targetPos.y - yOffset, 60, 20), _builtPercentage + "/" + 100);
            }
        }

        public string GetId()
        {
            return _id;
        }

        public void SetAvailable(bool isAvailable)
        {
            _isAvailable = isAvailable;
        }
        
        public bool IsAvailable()
        {
            return _isAvailable;
        }

        public void SetCurrentResource(Resource resource)
        {
            _currentResource = resource;

            _isAvailable = false;

            foreach (Unit worker in _workers)
            {
                worker.SetCurrentResource(_currentResource);
            }

            SetWorkersToWork();
        }
        
        public Resource GetCurrentResource()
        {
            return _currentResource;
        }
        
        private void BuildingOnTick()
        {
            if (_isBuilding)
            {
                int tempBuiltPercentage = _builtPercentage + _buildingConfig.constructPerTick;
                if (tempBuiltPercentage >= 100)
                {
                    _builtPercentage = 100;
                    _isBuilding = false;
                    _resourceSignals.FireRegisterBuilding( new RegisterBuildingSignal() { sender = this}) ;
                    _isAvailable = true;
            
                    SpawnWorker();
                }
                else
                {
                    _builtPercentage = tempBuiltPercentage;
                }
            }
            else
            {
                // do everything else
            }
        }

        private void SpawnWorker()
        {
            if (_buildingConfig.unit != null)
            {
                GameObject unitGb = (GameObject)Resources.Load(_buildingConfig.unit.path);
                Unit unit = _unitFactory.Create(unitGb, this.transform.parent.transform);

                Vector3 parentPosition = this.transform.position;
                
                unit.transform.position = parentPosition;
                unit.SetParentBuilding(this);
                _workers.Add(unit);
            }
        }

        private void SetWorkersToWork()
        {
            foreach (var worker in _workers)
            {
                worker.Work();
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