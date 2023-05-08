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
        public int gameMonth;

        public SaveStateMasterObject(List<BuildingSaveState> buildings, Dictionary<ResourceType, float> resources, int timeTick, int gameMonth)
        {
            this.buildings = buildings;
            this.timeTick = timeTick;
            this.resources = resources;
            this.gameMonth = gameMonth;
        }
        
    }
}