using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Enum;
using Common.Interface;
using Common.Logic;
using Game;
using ResourceNS;
using ResourceNS.Enum;
using Signals.ResourceNS;
using Signals.StockpileNS;
using Signals.UI;
using Ui.Common;
using UnitNS;
using Unity.VisualScripting;
using UnityEngine;
using Util;
using Zenject;
using Unit = UnitNS.Unit;

namespace BuildingNS
{
    public class PlaceableBuilding : MonoBehaviour, IViewableInfo
    {
        [Inject] private readonly UnitFactory _unitFactory;
        [Inject] protected readonly ResourceSignals _resourceSignals;
        [Inject] protected readonly StockpileSignals _stockpileSignals;
        [Inject] protected readonly UiSignals _uiSignals;
        [Inject] protected readonly OnClickHighlightLogic _onClickHighlightService;
        
        private int _builtPercentage = 0;
        private CfgBuilding _buildingConfig;
        private bool _isAvailable;
        private bool _isBuilding;
        private bool _isLoaded = false;
        private Camera _camera;
        
        private Resource _currentResource;
        private List<Unit> _workers; // todo change to array

        private string _id;

        private float _reservedResourceAmount;
        protected float _toProduceResourceAmount; // TODO: Should be config
        
        public bool canBuild;
 
        public ResourceType preferredResource;
        

        public virtual void Start()
        {
            _id = Guid.NewGuid().ToString();
            canBuild = true;
            _workers = new List<Unit>();
            _camera = Camera.main;
            _reservedResourceAmount = 0; // TODO: is it really 0?
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
            Debug.LogWarning("This is now deprecated since load state does not account for reserved resource");
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
            _isAvailable = true;
            _resourceSignals.FireRegisterBuilding( new RegisterBuildingSignal() { sender = this}) ;
            
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
                //Reason for this.transform.parent.transform is that unit cannot be a child of object it is colliding with
                // instead of FarmGameobject -> FarmCube -> Farmer
                // FarmGameobject -> FarmCube AND FarmGameobject -> Farmer
                Unit unit = _unitFactory.Create(unitGb, this.transform.parent.transform);
                // TODO hack 
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
            Debug.Log("This is base class, there is no resource for this building");
        }

        public virtual float GetResourceAmount()
        {
            Debug.Log("This is base class, there is no resource for this building");
            return 0f;
        }
        
        public string GetBuildingType()
        {
            return _buildingConfig.type;
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

        public void AddReservedResourceAmount(float amount)
        {
            this._reservedResourceAmount += amount;
        }
        
        public void DecreaseReservedResourceAmount(float amount)
        {
            this._reservedResourceAmount -= amount;
        }
        
        public void SetReservedResourceAmount(float amount)
        {
            this._reservedResourceAmount = amount;
        }
        
        public float GetReservedResourceAmount()
        {
            return this._reservedResourceAmount;
        }

        public float GetToProduceResourceAmount()
        {
            return this._toProduceResourceAmount;
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
        public UiBuildingInfo CreateUiBuildingInfo()
        {
            string resourceInfo = $"Available: {IsAvailable()}";
            string workerInfo = $"Total amount of workers: {_workers.Count}";
            Vector3? workerPos = null;
            
            // just position of any worker
            if (this._workers.Count > 0)
            {
                workerPos = this._workers.First().transform.position;
            }
            UiBuildingInfo info = new UiBuildingInfo(_id, _buildingConfig.name, GameEntityType.Building, workerInfo, resourceInfo, workerPos);
            return info;
        }

        /*
         * TRIGGERS
         */

        public void OnMouseDown()
        {
            _onClickHighlightService.Highlight(this.gameObject);
            UiBuildingInfo info = CreateUiBuildingInfo();
            BuildingInfoViewSignal signal = new BuildingInfoViewSignal{buildingInfo = info};
            _uiSignals.FireBuildingInfoViewSignal(signal);
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