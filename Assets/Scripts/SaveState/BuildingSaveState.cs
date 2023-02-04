using System;
using UnityEngine;
using Util;

namespace SaveStateNS
{
    [System.Serializable]
    public class BuildingSaveState
    {
        private Transform _position;
        private CfgBuilding _config;
        private String _id;

        public BuildingSaveState(Transform position, CfgBuilding config, String id)
        {
            _position = position;
            _config = config;
            _id = id;
        }
    }
}