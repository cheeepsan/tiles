using System;

namespace Game.BuildingNS
{
    public class BuildingPlaced : IBuildingSignal
    {
        public int id;
        public DateTime placedTimestamp;
    }
}