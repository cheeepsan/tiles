using System;
using ResourceNS.Enum;
using Signals.ResourceNS;
using Unity.VisualScripting;
using UnityEngine;

namespace BuildingNS
{
    public class Farm : PlaceableBuilding
    {
        public override void Start()
        {
            base.Start();
            preferredResource = ResourceType.Farm;
        }

        public override void DisposeResources(Tuple<ResourceType, float> resourceTuple)
        {
            _resourceSignals.FireAddResourceToQueue(new AddResourceToQueueSignal() {resource =  resourceTuple});
        }
    }
}