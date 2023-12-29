using Common.Enum;
using Common.Interface;
using ExceptionNS.Resource;
using ResourceNS.Enum;
using Signals.ResourceNS;
using Signals.UI;
using Ui.Common;
using UnitNS;
using UnityEngine;

namespace ResourceNS
{
    public enum FarmPlotStatus
    {
        Building,
        Ready,
        Depleted
    }
    
    public class FarmPlot : Resource, IViewableInfo
    {

        private Camera _camera;
        // TODO Move to conf
        private float _timeToBuild = 10;
        private float _timeToGather = 10;
        private float _constructPerTick = 10f;
        private float _yieldPerTick = 5f;
        private float _maxYield = 15;
        //
        private float _accumulatedYield = 0f;
        private float _builtPercentage = 0f;
        private FarmPlotStatus _farmPlotStatus;
        
        public override void Start()
        {
            base.Start();
            _camera = Camera.main;
            _farmPlotStatus = FarmPlotStatus.Building;
            resourceType = ResourceType.Farm;
            yield = 0f;
        }

        public override void ResourceHandling(Unit unit, float tick)
        {
            if (_farmPlotStatus == FarmPlotStatus.Building)
            {
                Debug.Log("Building the farm, tick: " + tick);
                float intermediateBuiltPercentage = _builtPercentage + _constructPerTick;
                if (intermediateBuiltPercentage >= 100)
                {
                    _builtPercentage = 100;
                    _farmPlotStatus = FarmPlotStatus.Ready;
                }
                else
                {
                    _builtPercentage = intermediateBuiltPercentage;
                }
            }
            else if (_farmPlotStatus == FarmPlotStatus.Ready)
            {
                float intermediateYield = _accumulatedYield + _yieldPerTick;
                if (intermediateYield >= _maxYield)
                {
                    float difference = _maxYield - _accumulatedYield;
                    if (difference > 0)
                    {
                        yield = difference;
                    }
                    
                    _resourceSignals.FireResourceDepleted(new ResourceDepleted(){depletedResource = this});
                    _farmPlotStatus = FarmPlotStatus.Depleted;
                }
                else
                {
                    yield += intermediateYield;
                }
            }
            else
            {
                Debug.Log("Farm ready to delete");
            }
        }

        public override float GetYield()
        {

            var possibleYield = 0f;
            if (yield.HasValue)
            {
                possibleYield = yield.Value;
            }
            
            float yieldForResource = _farmPlotStatus switch
            {
                FarmPlotStatus.Building => 0f,
                FarmPlotStatus.Ready => possibleYield,
                FarmPlotStatus.Depleted => 0f,
                _ => throw new ResourceNotAvailableException("Resource not available, type: FarmPlot, id: " + base.resourceUuid)
            };
            
            return yieldForResource;
        }
        
        

        public override float TimeToFinish()
        {

            float timeToReturn = _farmPlotStatus switch
            {
                FarmPlotStatus.Building => _timeToBuild,
                FarmPlotStatus.Ready => _timeToGather,
                FarmPlotStatus.Depleted => 0f,
                _ => throw new ResourceNotAvailableException("Resource not available, type: FarmPlot, id: " + base.resourceUuid)
            };
            
            return timeToReturn;
        }
        
        public void OnGUI()
        {

            Vector2 targetPos;
            targetPos = _camera.WorldToScreenPoint(transform.position);

            float yOffset = 60; // not fixed? 
            float xOffset = 30;
            if (_farmPlotStatus == FarmPlotStatus.Building)
            {
                GUI.Box(new Rect(targetPos.x - xOffset, Screen.height - targetPos.y - yOffset, 60, 20), _builtPercentage + "/" + 100);
            } else if (_farmPlotStatus == FarmPlotStatus.Ready)
            {
                GUI.Box(new Rect(targetPos.x - xOffset, Screen.height - targetPos.y - yOffset, 60, 20), _accumulatedYield + "/" + _maxYield);
            }
        }

        public override void ZeroYield()
        {
            _accumulatedYield += yield.Value;
            base.ZeroYield();

        }

        public UiBuildingInfo CreateUiBuildingInfo()
        {
            string resourceInfo = $"Available: {IsAvailable()}, yield: {this.yield}," +
                                  $" type: {this.resourceType}, Time to gather {this._timeToGather}" +
                                  $"max yield: {this._maxYield}";
            
            UiBuildingInfo info = new UiBuildingInfo(this.resourceUuid, this.resourceType.ToString(), GameEntityType.Resource, null, resourceInfo, null);
            return info;
        }
        
        public void OnDestroy()
        {
            
        }
        
        public void OnMouseDown()
        {
            Debug.Log("Clicked on: " + this.resourceUuid + ", type: " + this.resourceType.Value);
            UiBuildingInfo info = CreateUiBuildingInfo();
            BuildingInfoViewSignal signal = new BuildingInfoViewSignal{buildingInfo = info};
            _uiSignals.FireBuildingInfoViewSignal(signal);
        }
    }
}