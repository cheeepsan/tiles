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
        
        // TODO: DO BEFORE/AFTER Dispose in generic way
        public override void DisposeResources(Tuple<ResourceType, float> resourceTuple)
        {
            if (resourceTuple.Item2 > 0)
            {
                _stockpileSignals.FireAddResourceToQueue(new AddResourceToQueueSignal() {resource =  resourceTuple});
            }


            if (this.GetCurrentResource() != null)
            {
                this.GetCurrentResource().SetAvailable(true);
            }
            
            this.SetAvailable(true); // ?????
        }
    }
}