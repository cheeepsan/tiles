using System;
using Newtonsoft.Json;
using UnityEngine;
using Util;

namespace SaveStateNS
{
    [System.Serializable]
    public class BuildingSaveState
    {
        public float x;
        public float y;
        public float z;

        public CfgBuilding config;
        public String id;

        public BuildingSaveState(Transform transform, CfgBuilding config, String id)
        {
            Vector3 pos = transform.position;
            
            this.x = pos.x;
            this.y = pos.y;
            this.z = pos.z;

            this.config = config;
            this.id = id;
        }
        
        [JsonConstructor]
        public BuildingSaveState(float x, float y, float z, CfgBuilding config, String id)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            this.config = config;
            this.id = id;
        }
    }
}