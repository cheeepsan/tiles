using ExceptionNS.Resource;
using ResourceNS.Enum;
using UnitNS;
using UnityEngine;

namespace ResourceNS
{
    public enum FarmPlotStatus
    {
        Building,
        Ready
    }
    
    public class FarmPlot : Resource
    {

        private Camera _camera;
        // TODO Move to conf
        private float _timeToBuild = 10;
        private float _timeToGather = 10;
        private float _constructPerTick = 10f;
        private float _yieldPerTick = 5f;
        private float _maxYield = 420f;
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
            else
            {
                yield = yield.Value + _yieldPerTick;
            }
        }

        public override float GetYield()
        {
            
            float yieldForResource = _farmPlotStatus switch
            {
                FarmPlotStatus.Building => 0f,
                FarmPlotStatus.Ready => yield.Value,
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
            _accumulatedYield = _accumulatedYield + yield.Value;
            base.ZeroYield();

        }
        
        public void OnDestroy()
        {
            
        }
    }
}