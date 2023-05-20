
using ResourceNS.Enum;
using UnitNS;
using UnityEngine;

namespace ResourceNS
{
    public class Fruits : Resource
    {
        private int _timeToGather = 10;
        
        // TODO to config
        private float _yieldPerTick = 1f;
        
        public override void Start()
        {
            base.Start();
            resourceType = ResourceType.Fruits;
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

    }
}