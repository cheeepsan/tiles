
using Common.Enum;
using Common.Interface;
using ResourceNS.Enum;
using Signals.UI;
using Ui.Common;
using UnitNS;
using UnityEngine;

namespace ResourceNS
{
    public class Clay : Resource, IViewableInfo
    {
        private int _timeToGather = 10;
        
        // TODO to config
        private float _yieldPerTick = 1f;
        
        public override void Start()
        {
            base.Start();
            resourceType = ResourceType.Clay;
            yield = 0f;
            
        }
        
        public override float TimeToFinish()
        {
            return _timeToGather;
        }

        public override void ResourceHandling(Unit unit, float tick)
        {
            yield = yield.Value + _yieldPerTick;
        }

        public UiBuildingInfo CreateUiBuildingInfo()
        {
            string resourceInfo = $"Available: {IsAvailable()}, yield: {this.yield}," +
                                  $" type: {this.resourceType}, Time to gather {this._timeToGather}";
            UiBuildingInfo info = new UiBuildingInfo(this.resourceUuid, this.resourceType.ToString(),  GameEntityType.Resource, null, resourceInfo, null);
            return info;
        }

        public void OnMouseDown()
        {
            _onClickHighlight.Highlight(this.gameObject);
            UiBuildingInfo info = CreateUiBuildingInfo();
            BuildingInfoViewSignal signal = new BuildingInfoViewSignal{buildingInfo = info};
            _uiSignals.FireBuildingInfoViewSignal(signal);
        }
    }
}