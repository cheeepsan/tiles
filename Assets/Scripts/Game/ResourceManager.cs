using System;
using System.Collections.Generic;
using System.Linq;
using BuildingNS;
using ResourceNS;
using Signals.ResourceNS;
using UnitNS;

namespace Game
{
    /// <summary>
    ///  Keep track of available resource spots: fruits, available farms etc...
    /// </summary>
    public class ResourceManager
    {
        private readonly ResourceSignals _resourceSignals;

        private Dictionary<string, Resource> _resources;

        public ResourceManager(ResourceSignals resourceSignals)
        {
            _resourceSignals = resourceSignals;
            _resources = new Dictionary<string, Resource>();
            SubscribeToSignals();
        }

        private void SubscribeToSignals()
        {
            SubscribeToResourceAvailableSignal();
            SubscribeToAskForAvailableResourcesSignal();
        }


        private void SubscribeToAskForAvailableResourcesSignal()
        {
            Action<AskForAvailableResourceSignal, Dictionary<string, Resource>> EnableAvailableResource = (s, d) =>
            {
                Unit sender = s.sender;

                sender.SetCurrentResource(_resources.First().Value);

            };
            
            _resourceSignals.Subscribe2<AskForAvailableResourceSignal, Dictionary<string, Resource>>( (x, b) =>
                    EnableAvailableResource(x, b)
                , _resources);
        }
        
        private void SubscribeToResourceAvailableSignal()
        {
            Action<ResourceAvailableSignal, Dictionary<string, Resource>> ResourceAvailable = (s, d) =>
            {
                d.Add(s.resourceId, s.resource);
            };
            
            _resourceSignals.Subscribe2<ResourceAvailableSignal, Dictionary<string, Resource>>( (x, b) =>
                    ResourceAvailable(x, b)
                , _resources);
        }
        

    }
}