using System;
using BuildingNS;
using ResourceNS.Enum;

namespace Common.Signals
{
    public struct RetrieveResourceQueueSignalStruct
    {
        private Tuple<ResourceType, float> resource;
        private PlaceableBuilding building;

        public RetrieveResourceQueueSignalStruct(Tuple<ResourceType, float> resource, PlaceableBuilding building)
        {
            this.resource = resource;
            this.building = building;
        }

        public Tuple<ResourceType, float> GetResource()
        {
            return resource;
        }

        public PlaceableBuilding GetBuilding()
        {
            return building;
        }
    }
}