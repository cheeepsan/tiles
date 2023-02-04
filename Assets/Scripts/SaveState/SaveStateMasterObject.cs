using System.Collections.Generic;

namespace SaveStateNS
{   
    [System.Serializable]
    public class SaveStateMasterObject
    {
        private List<BuildingSaveState> _buildings;
        private int _timeTick;

        public SaveStateMasterObject(List<BuildingSaveState> buildings, int timeTick)
        {
            _buildings = buildings;
            _timeTick = timeTick;
        }
        
    }
}