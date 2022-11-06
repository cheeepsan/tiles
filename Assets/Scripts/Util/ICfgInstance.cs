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

        public override string ToString()
        {
            return $"id: {id}, name: {name}, path: {path}, type: {type}";
        }
    }
}