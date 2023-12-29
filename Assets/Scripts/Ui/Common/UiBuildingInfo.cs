using Common.Enum;
using JetBrains.Annotations;
using UnityEngine;

namespace Ui.Common
{
    public class UiBuildingInfo
    {
        public string id;
        public string name;
        public GameEntityType type;
        [CanBeNull] public string workerInfo;
        [CanBeNull] public string resourceInfo;
        [CanBeNull] public Vector3? workerPos;
        

        public UiBuildingInfo(string id, string name, GameEntityType type, string workerInfo, string resourceInfo, Vector3? workerPos)
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.workerInfo = workerInfo;
            this.resourceInfo = resourceInfo;
            this.workerPos = workerPos;
        }
    }
}