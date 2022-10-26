using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

namespace Util
{
    public class Configuration
    {
        private List<CfgBuilding> _buildings;

        public Configuration()
        {
            TextAsset buildingsConf = Resources.Load<TextAsset>("Config/building");

            _buildings = JsonConvert.DeserializeObject<List<CfgBuilding>>(buildingsConf.text);
        }

        public List<CfgBuilding> GetCfgBuildingList()
        {
            return this._buildings;
        }
    }
    

}