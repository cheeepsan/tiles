namespace Util
{
    public interface CfgInstance
    {
    }


    [System.Serializable]
    public class CfgBuilding : CfgInstance
    {
        public string id;
        public string name;
        public string path;
    }
}