using MSNTools.PersistentData;
using System;
using System.Collections.Generic;

namespace MSNTools.Functions
{
    [Serializable]
    public struct ShopStructure
    {
        private ItemDataSerializable ItemData;
        private int SellerEntityID;
        private string SellerName;
        private int Price;
        private int ID;
        private string buyerName;

        public ShopStructure(int id, int sellerEntityId, string sellerName, ItemDataSerializable itemData, int price)
        {
            ItemData = itemData;
            SellerEntityID = sellerEntityId;
            SellerName = sellerName;
            Price = price;
            ID = id;
            buyerName = "Unknown";
        }

        public string BuyerName
        {
            get
            {
                return buyerName;
            }
            set
            {
                buyerName = value;
            }
        }

        public int GetID
        {
            get
            {
                return ID;
            }
        }

        public int GetSellerEntityID
        {
            get
            {
                return SellerEntityID;
            }
        }

        public string GetItemName
        {
            get
            {
                return ItemData.name;
            }
        }

        public ItemDataSerializable GetItemDataSerializable
        {
            get
            {
                return ItemData;
            }
        }

        public string GetSellerName
        {
            get
            {
                return SellerName;
            }
        }

        public int GetPrice
        {
            get
            {
                return Price;
            }
        }

        public override string ToString()
        {
            return $"ItemStack :\n" +
                $"   name : {Localization.Get(GetItemName)}\n" +
                $"   count : {ItemData.count}\n" +
                $"   quality : {ItemData.quality}\n\n" +
                $"Owner :\n" +
                $"   name : {GetSellerName}\n\n" +
                $"Price : {Price}\n\n" +
                $"ID : {GetID}";
        }
    }
}