using System.Collections.Generic;
using ResourceNS;
using Signals.Resource;

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
        }
    }
}