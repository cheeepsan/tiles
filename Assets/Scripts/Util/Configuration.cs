using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

namespace Util
{
    public class Configuration
    {
        private List<CfgBuilding> _buildings;
        private List<CfgBuilding> _worldObjects;
        private Dictionary<CfgUiElementsEnum, CfgUi> _ui;

        public Configuration()
        {
            TextAsset buildingsConf = Resources.Load<TextAsset>("Config/building");
            _buildings = JsonConvert.DeserializeObject<List<CfgBuilding>>(buildingsConf.text);
            
            TextAsset worldObjectsConf = Resources.Load<TextAsset>("Config/world-objects");
            _worldObjects = JsonConvert.DeserializeObject<List<CfgBuilding>>(worldObjectsConf.text);
            
            TextAsset uiConf = Resources.Load<TextAsset>("Config/ui");
            _ui = JsonConvert.DeserializeObject<Dictionary<CfgUiElementsEnum, CfgUi>>(uiConf.text);
        }

        public List<CfgBuilding> GetCfgBuildingList()
        {
            return _buildings;
        }
        
        public List<CfgBuilding> GetCfgWorldObjectsList()
        {
            return _worldObjects;
        }
        
        public Dictionary<CfgUiElementsEnum, CfgUi> GetUIConfiguration()
        {
            return _ui;
        }
    }
    

}