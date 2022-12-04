using System.Collections.Generic;
using BuildingNS;
using ResourceNS;
using UnitNS;

namespace Signals.ResourceNS
{

    public class ResourceIsSetToBuildingSignal : IResourceSignal
    {
        public string buildingId;
    }
    
    public class RegisterBuildingSignal : IResourceSignal
    {
        public PlaceableBuilding sender;
    }
    
    public class ResponseForAvailableResourceSignal : IResourceSignal
    {
        public List<Resource> availableResources;
    }
}