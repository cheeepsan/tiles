using System.Collections.Generic;
using ResourceNS.Enum;
using Signals.ResourceNS;

namespace Signals.UI
{
    public class UpdateResourcesViewSignal : IResourceSignal
    {
        public Dictionary<ResourceType, float> resources;
    }
}