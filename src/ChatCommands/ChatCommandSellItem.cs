using MSNTools.Functions;
using MSNTools.PersistentData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MSNTools.ChatCommands
{
    public class ChatCommandSellItem : ChatCommandAbstract
    {
        private AutoTurretFireController.TurretEntitySorter sorter;

        public override void Execute(List<string> _params, ClientInfo _clientInfo)
        {
            try
            {
                if (!ChatCommandShop.IsEnabled)
                    return;
                if (_clientInfo != null)
                {
                    if (_params.Count == 1)
                    {
                        EntityPlayer entityPlayer = PersistentOperations.GetEntityPlayer(_clientInfo.entityId);
                        sorter = new AutoTurretFireController.TurretEntitySorter(entityPlayer.position);
                        int price = int.Parse(_params[0]);
                        var entities = GameManager.Instance.World.Entities.list;
                        var shopList = PersistentContainer.Instance.ShopList;
                        List<EntityItem> entityItems = new List<EntityItem>();
                        foreach (var entity in entities)
                        {
                            if (entity is EntityItem)
                            {
                                EntityItem entityItem = (EntityItem)entity;
                                entityItems.Add(entityItem);
                            }
                        }
                        if (entityItems.Count > 0)
                        {
                            ItemStack itemStack = entityItems[0].itemStack;
                            ItemDataSerializable itemData = new ItemDataSerializable()
                            {
                                name = itemStack.itemValue.ItemClass.Name,
                                cosmeticSlots = itemStack.itemValue.CosmeticMods.Length,
                                count = itemStack.count,
                                modSlots = itemStack.itemValue.Modifications.Length,
                                quality = itemStack.itemValue.Quality,
                                useTimes = itemStack.itemValue.UseTimes
                            };
                            entityItems.Sort(sorter);
                            ShopStructure shopStructure = new ShopStructure(EntityFactory.nextEntityID++, entityPlayer.entityId, entityPlayer.EntityName, itemData, price);
                            shopList.Add(shopStructure);
                            PersistentContainer.Instance.ShopList = shopList;
                            GameManager.Instance.World.RemoveEntity(entityItems[0].entityId, EnumRemoveEntityReason.Despawned);
                            PersistentContainer.DataChange = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "sell" };
    }
}