using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using ResourceNS;
using ResourceNS.Enum;
using Signals.ResourceNS;
using Signals.StockpileNS;
using UnitNS;
using UnityEngine;
using Util;
using Zenject;
namespace BuildingNS
{
    public class PlaceableBuilding : MonoBehaviour
    {
        [Inject] private readonly UnitFactory _unitFactory;
        [Inject] protected readonly ResourceSignals _resourceSignals;
        [Inject] protected readonly StockpileSignals _stockpileSignals;

        private int _builtPercentage = 0;
        private CfgBuilding _buildingConfig;
        private bool _isAvailable;
        private bool _isBuilding;
        private bool _isLoaded = false;
        private Camera _camera;
        
        private Resource _currentResource;
        private List<Unit> _workers; // todo change to array

        private string _id;
            
        public bool canBuild;
 
        public ResourceType preferredResource;
        

        public virtual void Start()
        {
            _id = Guid.NewGuid().ToString();
            canBuild = true;
            _workers = new List<Unit>();
            _camera = Camera.main;
            if (_isLoaded)
            {
                RestoredFromSaveState();
            }
        }
        /*
         * INIT 
         */
        private void RestoredFromSaveState()
        {
            InitiateOnBuilt();
            TimeManager.OnTick += delegate(object sender, TimeManager.OnTickEventArgs args)
            {
                BuildingOnTick();
            };
        }
        
        public void Place()
        {
            _isBuilding = true;
            TimeManager.OnTick += delegate(object sender, TimeManager.OnTickEventArgs args)
            {
                BuildingOnTick();
            };
        }
        
        private void InitiateOnBuilt()
        {
            _builtPercentage = 100;
            _isBuilding = false;
            _resourceSignals.FireRegisterBuilding( new RegisterBuildingSignal() { sender = this}) ;
            _isAvailable = true;
            
            SpawnWorker();
        }
        
        /*
         * FUNCTIONALITY
         */
        
        
        
        private void BuildingOnTick()
        {
            if (_isBuilding)
            {
                int tempBuiltPercentage = _builtPercentage + _buildingConfig.constructPerTick;
                if (tempBuiltPercentage >= 100)
                {
                    InitiateOnBuilt();
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

        public virtual void DisposeResources(Tuple<ResourceType, float> resourceTuple)
        {
            
        }

        /*
         * GET SET
         */
        
        public void SetBuildingConfig(CfgBuilding buildingCfg)
        {
            _buildingConfig = buildingCfg;
        }

        public CfgBuilding GetBuildingConfig()
        {
            return _buildingConfig;
        }

        public void SetId(string id)
        {
            _id = id;
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
        
        public void SetIsLoaded(bool isLoaded)
        {
            _isLoaded = isLoaded;
        }
        
        /*
         * OTHER
         */
        
        public void OnGUI()
        {

            Vector2 targetPos;
            targetPos = _camera.WorldToScreenPoint(transform.position);

            float yOffset = 60; // not fixed? 
            float xOffset = 30;
            if (_isBuilding)
            {
                GUI.Box(new Rect(targetPos.x - xOffset, Screen.height - targetPos.y - yOffset, 60, 20), _builtPercentage + "/" + 100);
            }
        }

        
        /*
         * TRIGGERS
         */
        
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