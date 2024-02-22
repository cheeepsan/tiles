using System;
using System.Linq;
using BuildingNS.Interface;
using Common.Enum;
using Game;
using ResourceNS.Enum;
using Signals.ResourceNS;
using Ui.Common;
using UnitNS;
using UnityEngine;
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
        
        public override UiBuildingInfo CreateUiBuildingInfo()
        {
            string resourceInfo = $"Available: {IsAvailable()}\n";
            string workerInfo = $"Total amount of workers: {workers.Count}\n" +
                                $"Reserved resource {preferredResource.ToString()} : {this.GetReservedResourceAmount()}";

            Vector3? workerPos = null;
            
            // just position of any worker
            if (this.workers.Count > 0)
            {
                workerPos = this.workers.First().transform.position;
            }
            UiBuildingInfo info = new UiBuildingInfo(_id, _buildingConfig.name, GameEntityType.Building, workerInfo, resourceInfo, workerPos);
            return info;
        }
    }
}