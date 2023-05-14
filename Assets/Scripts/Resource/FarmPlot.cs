using ResourceNS.Enum;

namespace ResourceNS
{
    public class FarmPlot : Resource
    {
        public override void Start()
        {
            base.Start();
            resourceType = ResourceType.Farm;
            yield = 5f;
        }
    }
}