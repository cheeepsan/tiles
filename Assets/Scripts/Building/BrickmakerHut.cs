using System;
using BuildingNS.Interface;
using Game;
using ResourceNS.Enum;
using Signals.ResourceNS;
using UnitNS;
using Zenject;

namespace BuildingNS
{
    public class BrickmakerHut : PlaceableBuilding, IProducingBuilding
    {
        [Inject] private StockpileManager _stockpileManager;

        public override void Start()
        {
            base.Start();
            _toProduceResourceAmount = 5f;
            preferredResource = ResourceType.Clay;
            TimeManager.On40Tick += delegate(object sender, TimeManager.On40TickEventArgs args)
            {
                PollToReserveResource();
            };
        }

        public override float GetResourceAmount()
        {
            return _stockpileManager.GetResourceAmount(this.preferredResource);
        }

        public void PollToReserveResource()
        {
            Tuple<ResourceType, float> r = new Tuple<ResourceType, float>(this.preferredResource, 5);
            _stockpileSignals.FireAddResourceRetrieveToQueue(new RetrieveResourceQueueSignal()
                { resource = r, building = this });
        }


        // TODO: DO BEFORE/AFTER Dispose in generic way
        public override void DisposeResources(Tuple<ResourceType, float> resourceTuple, Unit unit)
        {
            if (resourceTuple.Item2 > 0)
            {
                _stockpileSignals.FireAddResourceToQueue(new AddResourceToQueueSignal()
                {
                    resource =  resourceTuple, 
                    building = this,
                    unit = unit
                });
            }


            if (this.GetCurrentResource() != null)
            {
                this.GetCurrentResource().SetAvailable(true);
            }

            this.SetAvailable(true); // ?????
        }

        public override void ResourceStoredToStockpile(Unit unit)
        {
            
        }
    }
}