using MSNTools.Discord;
using System;
using System.Collections.Generic;

namespace MSNTools
{
    class InventoryChecks
    {
        public static bool IsEnabled, IsRunning, Invalid_Stack, Ban_Player, Check_Storage = false;
        public static int Admin_Level = 1;
        public static List<string> Exceptions_Items = new List<string> { "noteDuke01", "qtest_nextTraderAdmin" };

        public static void CheckInv(ClientInfo _cInfo, PlayerDataFile _playerDataFile)
        {
            try
            {
                if (_cInfo != null)
                {
                    if (GameManager.Instance.adminTools.GetUserPermissionLevel(_cInfo) > Admin_Level)
                    {
                        List<ItemStack> unauthorizedItems = new List<ItemStack>();
                        InventoryType inventoryType = InventoryType.None;
                        foreach (ItemStack item in _playerDataFile.inventory)
                        {
                            if (!item.IsEmpty() && item.itemValue.ItemClass.CreativeMode != EnumCreativeMode.All && item.itemValue.ItemClass.CreativeMode != EnumCreativeMode.Player && !Exceptions_Items.Contains(item.itemValue.ItemClass.Name))
                            {
                                unauthorizedItems.Add(item);
                                inventoryType = InventoryType.Toolbelt;
                            }
                        }
                        foreach (ItemStack item in _playerDataFile.bag)
                        {
                            if (!item.IsEmpty() && item.itemValue.ItemClass.CreativeMode != EnumCreativeMode.All && item.itemValue.ItemClass.CreativeMode != EnumCreativeMode.Player && !Exceptions_Items.Contains(item.itemValue.ItemClass.Name))
                            {
                                unauthorizedItems.Add(item);
                                inventoryType = InventoryType.Bag;
                            }
                        }
                        foreach (ItemValue item in _playerDataFile.equipment.GetItems())
                        {
                            if (!item.IsEmpty() && item.ItemClass.CreativeMode != EnumCreativeMode.All && item.ItemClass.CreativeMode != EnumCreativeMode.Player && !Exceptions_Items.Contains(item.ItemClass.Name))
                            {
                                unauthorizedItems.Add(new ItemStack(item, 1));
                                inventoryType = InventoryType.Equipment;
                            }
                        }
                        if (DiscordWebhookSender.SanctionsEnabled)
                        {
                            if (inventoryType != InventoryType.None)
                                SdtdConsole.Instance.ExecuteSync($"ban add {_cInfo.entityId} 10 years \"Caught with an unauthorized item\"", null);
                            if (inventoryType == InventoryType.Equipment)
                            {
                                DiscordWebhookSender.SendSanctionEmbedToWebHook(unauthorizedItems, _cInfo, $"Ban de {_cInfo.playerName}", "Détecté avec un ou plusieurs items équipés");
                            }
                            if (inventoryType == InventoryType.Toolbelt)
                            {
                                DiscordWebhookSender.SendSanctionEmbedToWebHook(unauthorizedItems, _cInfo, $"Ban de {_cInfo.playerName}", "Détecté avec un ou plusieurs items dans sa ceinture");
                            }
                            if (inventoryType == InventoryType.Bag)
                            {
                                DiscordWebhookSender.SendSanctionEmbedToWebHook(unauthorizedItems, _cInfo, $"Ban de {_cInfo.playerName}", "Détecté avec un ou plusieurs items dans son sac");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out($"{Config.ModPrefix} Error in InventoryChecks.CheckInv: {e.Message}");
            }
        }

        public static void CheckStoragesInLoadedChunks()
        {
            try
            {
                LinkedList<Chunk> _chunkArray = new LinkedList<Chunk>();
                DictionaryList<Vector3i, TileEntity> _tiles = new DictionaryList<Vector3i, TileEntity>();
                ChunkClusterList _chunklist = GameManager.Instance.World.ChunkClusters;
                for (int i = 0; i < _chunklist.Count; i++)
                {
                    ChunkCluster _chunk = _chunklist[i];
                    _chunkArray = _chunk.GetChunkArray();
                    foreach (Chunk _c in _chunkArray)
                    {
                        _tiles = _c.GetTileEntities();
                        if (_tiles.dict.Values != null)
                        {
                            foreach (TileEntity _tile in _tiles.dict.Values)
                            {
                                if (_tile == null)
                                    return;
                                if (_tile.GetTileEntityType().ToString().Contains("SecureLoot"))
                                {
                                    TileEntitySecureLootContainer SecureLoot = (TileEntitySecureLootContainer)_tile;
                                    if (SecureLoot.GetOwner() == null)
                                        continue;
                                    if (GameManager.Instance.adminTools.GetUserPermissionLevel(SecureLoot.GetOwner()) > Admin_Level)
                                    {
                                        ItemStack[] _items = SecureLoot.items;
                                        List<ItemStack> unauthorizedItems = new List<ItemStack>();
                                        for (int u = 0; u < _items.Length; u++)
                                        {
                                            if (!_items[u].IsEmpty())
                                            {
                                                if (_items[u].itemValue.ItemClass.CreativeMode != EnumCreativeMode.All && _items[u].itemValue.ItemClass.CreativeMode != EnumCreativeMode.Player && !Exceptions_Items.Contains(_items[u].itemValue.ItemClass.Name))
                                                {
                                                    unauthorizedItems.Add(_items[u]);
                                                    ItemStack itemStack = new ItemStack();
                                                    SecureLoot.UpdateSlot(u, itemStack.Clone());
                                                    _tile.SetModified();
                                                    Vector3i _chestPos = SecureLoot.ToWorldPos();
                                                }
                                            }
                                        }
                                        if (unauthorizedItems.Count > 0)
                                        {
                                            if (DiscordWebhookSender.AlertsEnabled)
                                            {
                                                DiscordWebhookSender.SendAlertEmbedToWebHook(unauthorizedItems, SecureLoot);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out($"{Config.ModPrefix} Error in InventoryChecks.CheckStoragesInLoadedChunks: {e.Message}");
            }
        }

        private enum InventoryType
        {
            Toolbelt,
            Bag,
            Equipment,
            None
        }
    }    
}