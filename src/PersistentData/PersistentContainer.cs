using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using JetBrains.Annotations;
using MSNTools.PersistentData;

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
        private Dictionary<int, int> auctionPrices;
        private Dictionary<int, int> backpacks;
        private Dictionary<int, List<int>> clientMuteList;
        private DateTime lastWeather;
        private List<string> protectedZones;
        private List<string> regionReset;

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
                Log.Out($"{Config.ModPrefix} Exception in PersistentContainer.Save: {e.Message}");
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
                Log.Out($"{Config.ModPrefix} Exception in PersistentContainer.Load: {e.Message}");
            }
            return false;
        }

        public Dictionary<int, int> AuctionPrices
        {
            get
            {
                return auctionPrices;
            }
            set
            {
                auctionPrices = value;
            }
        }

        public Dictionary<int, int> Backpacks
        {
            get
            {
                return backpacks;
            }
            set
            {
                backpacks = value;
            }
        }

        public Dictionary<int, List<int>> ClientMuteList
        {
            get
            {
                return clientMuteList;
            }
            set
            {
                clientMuteList = value;
            }
        }

        public DateTime LastWeather
        {
            get
            {
                return lastWeather;
            }
            set
            {
                lastWeather = value;
            }
        }

        public List<string> ProtectedZones
        {
            get
            {
                return protectedZones;
            }
            set
            {
                protectedZones = value;
            }
        }

        public List<string> RegionReset
        {
            get
            {
                return regionReset;
            }
            set
            {
                regionReset = value;
            }
        }
    }
}
