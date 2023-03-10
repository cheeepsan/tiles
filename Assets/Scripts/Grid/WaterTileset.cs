using System;
using System.Collections.Generic;
using System.Linq;
using Constant;
using UnityEngine;


namespace GridNS
{
    struct WaterTileData
    {
        public string guid;
        public float x, y, z;
    }
    public class WaterTileset
    {
        
        private Dictionary<Vector3, WaterTileData> _waterTileDatas;

        public WaterTileset()
        {
            _waterTileDatas = new Dictionary<Vector3, WaterTileData>();
            
            Grid grid = GameObject.FindObjectOfType<Grid>();
            Transform[] tiles = 
                grid.ground.GetComponentsInChildren<Transform>()
                    .Where(t => t.name == NameConstants.WATER_TILE)
                    .ToArray(); // TODO do smarter

            foreach (var transform in tiles)
            {
                String tileGuid = Guid.NewGuid().ToString();
                Vector3 pos = transform.position;
                WaterTileData tileData = new WaterTileData()
                {
                    guid = tileGuid,
                    x = pos.x,
                    y = pos.y,
                    z = pos.z
                };
                
                _waterTileDatas.Add(pos, tileData);
            }
        }
        /*
         * Object should be always in map, thus no check needed. If it fails, it's for a reason
         */
        public String GetWaterTileByPosition(Vector3 pos)
        {
            return _waterTileDatas[pos].guid;
        }

        public List<Vector3> GetAllWaterPos()
        {
            return _waterTileDatas.Select(x => x.Key).ToList();
        }
    }
}