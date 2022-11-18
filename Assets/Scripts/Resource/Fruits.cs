
using ResourceNS.Enum;

namespace ResourceNS
{
    public class Fruits : Resource
    {
        public override void Start()
        {
            base.Start();
            resourceType = ResourceType.Fruits;
        }
    }
}