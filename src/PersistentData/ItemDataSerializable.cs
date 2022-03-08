using System;

namespace MSNTools.PersistentData
{
    [Serializable]
    public class ItemDataSerializable
    {
        public ItemDataSerializable()
        {
            name = "";
            count = 0;
            useTimes = 0f;
            quality = 0;
            modSlots = 0;
            cosmeticSlots = 0;
        }

        public string name;

        public int count;

        public float useTimes;

        public int quality;

        public int modSlots;

        public int cosmeticSlots;
    }
}