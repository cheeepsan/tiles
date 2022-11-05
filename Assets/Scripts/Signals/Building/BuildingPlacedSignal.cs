using System;
using UnityEngine;
using Util;

namespace Game.BuildingNS
{
    public class BuildingPlacedSignal: IBuildingSignal
    {
        public DateTime placedTimestamp;
        public CfgBuilding buildingConfig;
        public Vector3 pos;
    }
}