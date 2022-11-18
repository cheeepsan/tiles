using System.Collections.Generic;
using ResourceNS;
using UnitNS;

namespace Signals.ResourceNS
{
    public class AskForAvailableResourceSignal : IResourceSignal
    {
        public Unit sender;
    }

    public class ResponseForAvailableResourceSignal : IResourceSignal
    {
        public List<Resource> availableResources;
    }
}