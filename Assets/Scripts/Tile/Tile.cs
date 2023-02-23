using System;
using UnityEngine;

namespace TileNS
{
    // TODO: CHANGE TO SCRIPTABLE OBJECT
    public class Tile : MonoBehaviour
    {
        private String _guid;
        public void OnEnable()
        {
            _guid = Guid.NewGuid().ToString();
        }

        public String GetGuid()
        {
            return _guid;
        }
    }
}