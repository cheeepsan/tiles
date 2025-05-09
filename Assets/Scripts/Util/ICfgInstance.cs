using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Util
{
    public interface CfgInstance
    {
    }


    [System.Serializable]
    public class CfgBuilding : CfgInstance
    {
        public int id;
        public string name;
        public string path;
        public string type;
        public int constructPerTick;
        [CanBeNull] public CfgUnit unit;
        [CanBeNull] public List<int> prerequisite;
        public override string ToString()
        {
            return $"id: {id}, name: {name}, path: {path}, type: {type}";
        }
    }
    
    [System.Serializable]
    public class CfgUnit : CfgInstance
    {
        public string name;
        public string path;
    }
    [System.Serializable]
    public class Settings : CfgInstance
    {
        public int tickPerMonth;
        public String tideMeshPath;
    }
}