using System;
using BuildingNS;
using JetBrains.Annotations;
using ResourceNS.Enum;
using UnitNS;

namespace Common.Signals
{
    // TODO create one more without unit
    public struct ResourceOperationQueueSignalStruct
    {
        private Tuple<ResourceType, float> resource;
        private PlaceableBuilding building;
        [CanBeNull] private Unit unit;

        
        public ResourceOperationQueueSignalStruct(Tuple<ResourceType, float> resource, PlaceableBuilding building)
        {
            this.resource = resource;
            this.building = building;
            this.unit = null;
        }
        
        public ResourceOperationQueueSignalStruct(Tuple<ResourceType, float> resource, PlaceableBuilding building, Unit unit)
        {
            this.resource = resource;
            this.building = building;
            this.unit = unit;
        }

        public Tuple<ResourceType, float> GetResource()
        {
            return resource;
        }

        public PlaceableBuilding GetBuilding()
        {
            return building;
        }

        public Unit GetUnit()
        {
            return unit;
        }
    }
}