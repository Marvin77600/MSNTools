using MSNTools.PersistentData;
using MSNTools.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MSNTools.ChatCommands
{
    public class ChatCommandBuy : ChatCommandAbstract
    {
        public override void Execute(List<string> _params, ClientInfo _clientInfo)
        {
            try
            {
                if (_clientInfo != null)
                {
                    if (_params.Count == 1)
                    {
                        int sellID = int.Parse(_params[0]);
                        EntityPlayer entityPlayer = PersistentOperations.GetEntityPlayer(_clientInfo.entityId);
                        var shopList = PersistentContainer.Instance.ShopList;
                        ItemStack itemStack = null;
                        
                        ShopStructure shopStructure = new ShopStructure();
                        for (int i = 0; i < shopList.Count; i++)
                        {
                            ShopStructure item = shopList[i];
                            ItemValue itemValue = ItemClass.GetItem(item.GetItemName, true);
                            if (sellID == item.GetID)
                            {
                                itemStack = new ItemStack(new ItemValue(itemValue.type, item.GetItemDataSerializable.quality, item.GetItemDataSerializable.quality, false), 1);
                                itemStack.itemValue.UseTimes = item.GetItemDataSerializable.useTimes;
                                shopStructure = item;
                                continue;
                            }
                        }
                        if (Bank.HasEnoughMoney(_clientInfo, shopStructure.GetPrice))
                        {
                            PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].PlayerWallet -= shopStructure.GetPrice;
                            shopList.Remove(shopStructure);
                            shopStructure.BuyerName = _clientInfo.playerName;
                            shopList.Add(shopStructure);
                            PersistentContainer.Instance.ShopList = shopList;
                            EntityItem entityItem = (EntityItem)EntityFactory.CreateEntity(new EntityCreationData
                            {
                                entityClass = EntityClass.FromString("item"),
                                id = EntityFactory.nextEntityID++,
                                itemStack = itemStack,
                                pos = entityPlayer.position,
                                rot = new Vector3(20f, 0f, 20f),
                                lifetime = 60f,
                                belongsPlayerId = _clientInfo.entityId
                            });
                            GameManager.Instance.World.SpawnEntityInWorld(entityItem);
                            _clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(entityItem.entityId, _clientInfo.entityId));
                            GameManager.Instance.World.RemoveEntity(entityItem.entityId, EnumRemoveEntityReason.Despawned);
                            ChatCommandsHook.ChatMessage(_clientInfo, $"Achat de {Localization.Get(shopStructure.GetItemName)} réussi ", -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                            PersistentContainer.DataChange = true;
                            PaySeller(shopStructure, _clientInfo, shopStructure.GetSellerEntityID);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public static void NotifSellerWhenConnected(ClientInfo sellerClientInfo)
        {
            if (sellerClientInfo != null)
            {
                var shopList = PersistentContainer.Instance.ShopList;
                for (int i = 0; i < shopList.Count; i++)
                {
                    ShopStructure item = shopList[i];
                    if (item.BuyerName != "Unknown" && item.GetSellerEntityID == sellerClientInfo.entityId)
                    {
                        ChatCommandsHook.ChatMessage(sellerClientInfo, $"{item.BuyerName} t'a acheté {Localization.Get(item.GetItemName)} !", -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                        PersistentContainer.Instance.ShopList.RemoveAt(i);
                        PersistentContainer.DataChange = true;
                    }
                }
            }
        }
        
        public void PaySeller(ShopStructure shopStructure, ClientInfo buyClientInfo, int entityID)
        {
            ClientInfo sellerClientInfo = PersistentOperations.GetClientInfoFromEntityId(entityID);
            if (sellerClientInfo != null)
            {
                PersistentContainer.Instance.Players[sellerClientInfo.PlatformId.ToString()].PlayerWallet += shopStructure.GetPrice;
                PersistentContainer.DataChange = true;
            }
            foreach (ClientInfo cInfo in PersistentOperations.ClientList())
            {
                if (cInfo.PlatformId.ToString() == sellerClientInfo.PlatformId.ToString())
                {
                    ChatCommandsHook.ChatMessage(sellerClientInfo, $"{buyClientInfo.playerName} vient de t'acheter {Localization.Get(shopStructure.GetItemName)} !", -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                }
            }
        }

        public override string[] GetCommands() => new string[] { "buy" };
    }
}