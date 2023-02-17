using System;
using System.Collections.Generic;
using ResourceNS.Enum;

namespace SaveStateNS
{
    [Serializable]
    public class SaveStateMasterObject
    {
        
        public List<BuildingSaveState> buildings;
        public Dictionary<ResourceType, float> resources;
        public int timeTick;

        public SaveStateMasterObject(List<BuildingSaveState> buildings, Dictionary<ResourceType, float> resources, int timeTick)
        {
            this.buildings = buildings;
            this.timeTick = timeTick;
            this.resources = resources;
        }
        
    }
}