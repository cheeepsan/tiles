using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Util
{
    public class Configuration
    {
        private List<CfgBuilding> _buildings;
        private List<CfgBuilding> _worldObjects;
        private Settings _settings;

        public Configuration()
        {
            TextAsset buildingsConf = Resources.Load<TextAsset>("Config/building");
            _buildings = JsonConvert.DeserializeObject<List<CfgBuilding>>(buildingsConf.text);
            
            TextAsset worldObjectsConf = Resources.Load<TextAsset>("Config/world-objects");
            _worldObjects = JsonConvert.DeserializeObject<List<CfgBuilding>>(worldObjectsConf.text);
            
            TextAsset settingsConf = Resources.Load<TextAsset>("Config/settings");
            _settings = JsonConvert.DeserializeObject<Settings>(settingsConf.text);
        }

        public Settings GetSettings()
        {
            return _settings;
        }
        
        public List<CfgBuilding> GetCfgBuildingList()
        {
            return _buildings;
        }
        
        public List<CfgBuilding> GetCfgWorldObjectsList()
        {
            return _worldObjects;
        }
    }
    

}