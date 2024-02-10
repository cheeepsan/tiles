using System;
using System.Collections.Generic;
using BuildingNS;
using ResourceNS;
using ResourceNS.Enum;
using UnitNS;
using UnityEngine;

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
    
    public class AddResourceToQueueSignal : IResourceSignal
    {
        public Tuple<ResourceType, float> resource;
    }
    
    public class RetrieveResourceQueueSignal : IResourceSignal
    {
        public Tuple<ResourceType, float> resource;
        public PlaceableBuilding building;
    }
    
    public class AddAvailableFarmPlotSignal : IResourceSignal
    {
        public Transform farmPlotTransform;
    }
    
    // TODO check naming, why not Signal
    public class ResourceDepleted : IResourceSignal
    {
        public Resource depletedResource;
    }
}