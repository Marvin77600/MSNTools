using MSNTools.Discord;
using MSNTools.PersistentData;
using System.Collections.Generic;
using UnityEngine;

namespace MSNTools
{
    public class Shop
    {
        public static bool IsEnabled = false;
        public static List<Item> ShopItems = new List<Item>();

        public class Item
        {
            private readonly string itemName;
            private readonly string description;
            private readonly int count;
            private readonly int quality;
            private readonly int price;
            private readonly int id;

            public Item(string _itemName, string _description, int _count, int _quality, int _price, int _id)
            {
                itemName = _itemName;
                count = _count;
                description = _description;
                quality = _quality;
                price = _price;
                id = _id;
            }

            public string ItemName { get => itemName; }

            public string Description { get => description; }

            public int Count { get => count; }

            public int Quality { get => quality; }

            public int Price { get => price; }

            public int ID { get => id; }
            
            public void Purchase(ClientInfo _clientInfo)
            {
                if (_clientInfo != null)
                {
                    if (Bank.HasEnoughMoney(_clientInfo, price))
                    {
                        EntityPlayer entityPlayer = GameManager.Instance.World.Players.dict[_clientInfo.entityId];
                        ItemValue itemValue = new ItemValue(ItemClass.GetItem(itemName).type, quality, quality);
                        if (quality != 0)
                        {
                            if (quality - 2 < 0) itemValue.Modifications = new ItemValue[1];
                            else itemValue.Modifications = new ItemValue[quality - 2];
                            itemValue.Quality = quality;
                            itemValue.CosmeticMods = new ItemValue[1];
                        }
                        EntityItem entityItem = (EntityItem)EntityFactory.CreateEntity(new EntityCreationData
                        {
                            entityClass = EntityClass.FromString("item"),
                            id = EntityFactory.nextEntityID++,
                            itemStack = new ItemStack(itemValue, count),
                            pos = entityPlayer.position,
                            rot = new Vector3(20f, 0f, 20f),
                            lifetime = 60f,
                            belongsPlayerId = _clientInfo.entityId
                        });
                        PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].PlayerWallet -= price;
                        PersistentContainer.DataChange = true;
                        GameManager.Instance.World.SpawnEntityInWorld(entityItem);
                        _clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(entityItem.entityId, _clientInfo.entityId));
                        GameManager.Instance.World.RemoveEntity(entityItem.entityId, EnumRemoveEntityReason.Despawned);
                        DiscordWebhookSender.SendNotifShop(_clientInfo, this);
                    }
                }
            }
        }
    }
}