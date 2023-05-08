using System;
using System.Collections.Generic;
using System.Linq;
using Constant;
using UnityEngine;

namespace GridNS
{
    struct GroundTileData
    {
        public string guid;
        public float x, y, z;
    }
    
    public class GroundTileset
    {

        private Dictionary<Vector3, GroundTileData> _groundTileDatas;

        public GroundTileset()
        {
            _groundTileDatas = new Dictionary<Vector3, GroundTileData>();
            
            Grid grid = GameObject.FindObjectOfType<Grid>();
            Transform[] tiles = 
                grid.ground.GetComponentsInChildren<Transform>()
                    .Where(t => t.name == NameConstants.GROUND_TILE)
                    .ToArray(); // TODO do smarter

            foreach (var transform in tiles)
            {
                String tileGuid = Guid.NewGuid().ToString();
                Vector3 pos = transform.position;
                GroundTileData tileData = new GroundTileData()
                {
                    guid = tileGuid,
                    x = pos.x,
                    y = pos.y,
                    z = pos.z
                };
                
                _groundTileDatas.Add(pos, tileData);
            }
        }
        /*
         * Object should be always in map, thus no check needed. If it fails, it's for a reason
         */
        public String GetGroundTileByPosition(Vector3 pos)
        {
            return _groundTileDatas[pos].guid;
        }
    }
}