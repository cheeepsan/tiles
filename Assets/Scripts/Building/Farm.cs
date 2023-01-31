using System;
using ResourceNS.Enum;
using Signals.ResourceNS;
using UnityEngine;

namespace BuildingNS
{
    public class Farm : PlaceableBuilding
    {
        public override void DisposeResources(Tuple<ResourceType, float> resourceTuple)
        {
            _resourceSignals.FireAddResourceToQueue(new AddResourceToQueueSignal() {resource =  resourceTuple});
        }
    }
}