using System;
using System.Collections.Generic;
using MSNTools.Discord;

namespace MSNTools
{
    class InventoryChecks
    {
        public static bool IsEnabled, IsRunning, Invalid_Stack, Ban_Player, Check_Storage = false;
        public static int Admin_Level = 1;
        private static List<String> exceptions = new List<string> { "noteDuke01" };

        public static void CheckInv(ClientInfo _cInfo, PlayerDataFile _playerDataFile)
        {
            try
            {
                if (_cInfo != null)
                {
                    if (GameManager.Instance.adminTools.GetUserPermissionLevel(_cInfo) > Admin_Level)
                    {
                        foreach (ItemStack item in _playerDataFile.inventory)
                        {
                            if (!item.IsEmpty() && item.itemValue.ItemClass.CreativeMode != EnumCreativeMode.All && item.itemValue.ItemClass.CreativeMode != EnumCreativeMode.Player && !exceptions.Contains(item.itemValue.ItemClass.Name))
                            {
                                SdtdConsole.Instance.ExecuteSync($"ban add {_cInfo.playerId} 10 years \"Caught with an unauthorized item: {item.itemValue.ItemClass.GetLocalizedItemName()}\"", null);
                                if (DiscordWebhookSender.SanctionsEnabled)
                                    DiscordWebhookSender.SendSanctionToWebHook(_cInfo, $"Ban de {_cInfo.playerName}", DiscordWebhookSender.SanctionsColor, $"Détecté en possession d'un item interdit dans sa ceinture: {item.itemValue.ItemClass.GetLocalizedItemName()}");
                                //GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, $"{_cInfo.playerName} a un item interdit dans sa ceinture ! ({item.itemValue.ItemClass.GetLocalizedItemName()})", "AntiCheat", false, null);
                            }
                        }
                        foreach (ItemStack item in _playerDataFile.bag)
                        {
                            if (!item.IsEmpty() && item.itemValue.ItemClass.CreativeMode != EnumCreativeMode.All && item.itemValue.ItemClass.CreativeMode != EnumCreativeMode.Player && !exceptions.Contains(item.itemValue.ItemClass.Name))
                            {
                                SdtdConsole.Instance.ExecuteSync($"ban add {_cInfo.playerId} 10 years \"Caught with an unauthorized item: {item.itemValue.ItemClass.GetLocalizedItemName()}\"", null);
                                if (DiscordWebhookSender.SanctionsEnabled)
                                    DiscordWebhookSender.SendSanctionToWebHook(_cInfo, $"Ban de {_cInfo.playerName}", DiscordWebhookSender.SanctionsColor, $"Détecté en possession d'un item interdit dans son sac: {item.itemValue.ItemClass.GetLocalizedItemName()}");
                                //GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, $"{_cInfo.playerName} a un item interdit dans son sac ! ({item.itemValue.ItemClass.GetLocalizedItemName()})", "AntiCheat", false, null);
                            }
                        }
                        foreach (ItemValue item in _playerDataFile.equipment.GetItems())
                        {
                            if (!item.IsEmpty() && item.ItemClass.CreativeMode != EnumCreativeMode.All && item.ItemClass.CreativeMode != EnumCreativeMode.Player && !exceptions.Contains(item.ItemClass.Name))
                            {
                                SdtdConsole.Instance.ExecuteSync($"ban add {_cInfo.playerId} 10 years \"Caught with an unauthorized item: {item.ItemClass.GetLocalizedItemName()}\"", null);
                                if (DiscordWebhookSender.SanctionsEnabled)
                                    DiscordWebhookSender.SendSanctionToWebHook(_cInfo, $"Ban de {_cInfo.playerName}", DiscordWebhookSender.SanctionsColor, $"Détecté avec un item interdit équipé: {item.ItemClass.GetLocalizedItemName()}");
                                //GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, $"{_cInfo.playerName} a un item interdit d'équipé ! ({item.ItemClass.GetLocalizedItemName()})", "AntiCheat", false, null);
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
                        foreach (TileEntity _tile in _tiles.dict.Values)
                        {
                            if (_tile.GetTileEntityType().ToString().Contains("SecureLoot"))
                            {
                                TileEntitySecureLootContainer SecureLoot = (TileEntitySecureLootContainer)_tile;
                                if (GameManager.Instance.adminTools.GetUserPermissionLevel(SecureLoot.GetOwner()) > Admin_Level)
                                {
                                    ItemStack[] _items = SecureLoot.items;
                                    int slotNumber = 0;
                                    foreach (ItemStack _item in _items)
                                    {
                                        if (!_item.IsEmpty())
                                        {
                                            if ((!_item.IsEmpty() && _item.itemValue.ItemClass.CreativeMode != EnumCreativeMode.All && _item.itemValue.ItemClass.CreativeMode != EnumCreativeMode.Player && !exceptions.Contains(_item.itemValue.ItemClass.Name)))
                                            {
                                                string _itemName = ItemClass.list[_item.itemValue.type].Name;
                                                ItemStack itemStack = new ItemStack();
                                                SecureLoot.UpdateSlot(slotNumber, itemStack.Clone());
                                                _tile.SetModified();
                                                Vector3i _chestPos = SecureLoot.localChunkPos;
                                                Log.Out($"{Config.ModPrefix} Removed {_item.count} {_itemName}, from a secure loot located at {_chestPos.x} {_chestPos.y} {_chestPos.z}, owned by {SecureLoot.GetOwner()}");
                                            }
                                        }
                                        slotNumber++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out($"{Config.ModPrefix} Error in InvalidItems.CheckStoragesInLoadedChunks: {e.Message}");
            }
        }
    }
}
