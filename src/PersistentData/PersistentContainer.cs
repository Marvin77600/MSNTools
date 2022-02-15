using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MSNTools.PersistentData
{
    [Serializable]
    public class PersistentContainer
    {
        public static bool DataChange = false;

        private static string filepath = string.Format("{0}/MSNTools.bin", Config.ConfigPath);
        private static PersistentContainer instance;
        private PersistentPlayers players;
        private static bool Saving = false;
        private List<string> protectedZones;
        private List<string> regionReset;
        private DateTime timeResetRegionFiles;
        private Dictionary<string, string[]> serverLocations;

        public static PersistentContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PersistentContainer();
                }
                return instance;
            }
        }

        public PersistentPlayers Players
        {
            get
            {
                if (players == null)
                {
                    players = new PersistentPlayers();
                }
                return players;
            }
        }

        private PersistentContainer()
        {
        }

        public void Save()
        {
            try
            {
                if (DataChange)
                {
                    if (!Saving)
                    {
                        Saving = true;
                        Stream stream = File.Open(filepath, FileMode.Create, FileAccess.ReadWrite);
                        BinaryFormatter bFormatter = new BinaryFormatter();
                        bFormatter.Serialize(stream, this);
                        stream.Close();
                        stream.Dispose();
                        Saving = false;
                    }
                    DataChange = false;
                }
            }
            catch (Exception e)
            {
                Saving = false;
                MSNUtils.LogError($"Exception in PersistentContainer.Save: {e.Message}");
            }
        }

        public bool Load()
        {
            try
            {
                if (File.Exists(filepath))
                {
                    PersistentContainer obj;
                    Stream stream = File.Open(filepath, FileMode.Open, FileAccess.Read);
                    BinaryFormatter bFormatter = new BinaryFormatter();
                    obj = (PersistentContainer)bFormatter.Deserialize(stream);
                    stream.Close();
                    stream.Dispose();
                    instance = obj;
                    return true;
                }
                else
                {
                    Stream stream = File.Open(filepath, FileMode.Create, FileAccess.ReadWrite);
                    BinaryFormatter bFormatter = new BinaryFormatter();
                    bFormatter.Serialize(stream, this);
                    stream.Close();
                    stream.Dispose();
                    return true;
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Exception in PersistentContainer.Load: {e.Message}");
            }
            return false;
        }

        public Dictionary<string, string[]> ServerLocations
        {
            get
            {
                if (serverLocations == null)
                {
                    serverLocations = new Dictionary<string, string[]>();
                }
                return serverLocations;
            }
            set
            {
                serverLocations = value;
            }
        }

        public DateTime TimeRegionFiles
        {
            get
            {
                return timeResetRegionFiles;
            }
            set
            {
                timeResetRegionFiles = value;
            }
        }

        public List<string> ProtectedZones
        {
            get
            {
                if (protectedZones == null)
                {
                    protectedZones = new List<string>();
                }
                return protectedZones;
            }
            set
            {
                protectedZones = value;
            }
        }

        public List<string> RegionsReset
        {
            get
            {
                if (regionReset == null)
                {
                    regionReset = new List<string>();
                }
                return regionReset;
            }
            set
            {
                regionReset = value;
            }
        }
    }
}
