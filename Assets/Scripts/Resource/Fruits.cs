
using ResourceNS.Enum;
using UnitNS;

namespace ResourceNS
{
    public class Fruits : Resource
    {
        public override void Start()
        {
            base.Start();
            resourceType = ResourceType.Fruits;
            yield = 5f;
        }

        public override void ResourceHandling(Unit unit)
        {
            
        }
    }
}