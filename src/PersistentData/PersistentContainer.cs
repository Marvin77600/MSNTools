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
        public MSNLocalization.Language ServerLanguage;

        private static string filepath = string.Format("{0}/MSNTools.bin", Config.ConfigPath);
        private static PersistentContainer instance;
        private PersistentPlayers players;
        private bool Saving = false;
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
                        MSNUtils.LogWarning("PersistentContainer.Save() 1");
                        Saving = true;
                        MSNUtils.LogWarning("PersistentContainer.Save() 2");
                        using (Stream stream = File.Open(filepath, FileMode.Create, FileAccess.ReadWrite))
                        {
                            MSNUtils.LogWarning("PersistentContainer.Save() 3");
                            BinaryFormatter bFormatter = new BinaryFormatter();
                            MSNUtils.LogWarning("PersistentContainer.Save() 4");
                            bFormatter.Serialize(stream, this);
                            MSNUtils.LogWarning("PersistentContainer.Save() 5");
                        }
                    }
                    DataChange = false;
                    MSNUtils.LogWarning("PersistentContainer.Save() 6");
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Exception in PersistentContainer.Save: {e.Message}");
            }
            Saving = false;
        }

        public bool Load()
        {
            try
            {
                if (File.Exists(filepath))
                {
                    PersistentContainer obj;
                    using (Stream stream = File.Open(filepath, FileMode.Open, FileAccess.Read))
                    {
                        BinaryFormatter bFormatter = new BinaryFormatter();
                        obj = (PersistentContainer)bFormatter.Deserialize(stream);
                    }
                    instance = obj;
                    return true;
                }
                else
                {
                    using (Stream stream = File.Open(filepath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        BinaryFormatter bFormatter = new BinaryFormatter();
                        bFormatter.Serialize(stream, this);
                    }
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