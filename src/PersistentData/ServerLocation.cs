namespace MSNTools.PersistentData
{
    public class ServerLocation
    {
        private string name;
        private string[] pos;

        public ServerLocation(string _name, string[] _pos)
        {
            name = _name;
            pos = _pos;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string[] Position
        {
            get
            {
                return pos;
            }
        }
    }
}
