using System;
using BuildingNS;
using UnityEngine;
using Util;

namespace Signals.Building
{
    public class BuildingPlacedSignal: IBuildingSignal
    {
        public DateTime placedTimestamp;
        public PlaceableBuilding placeableBuilding;
    }
}