using ResourceNS;
using UnityEngine;

namespace Signals.ResourceNS
{
    public class ResourceAvailableSignal : IResourceSignal
    {
        public string resourceId;
        public Resource resource;
    }
}