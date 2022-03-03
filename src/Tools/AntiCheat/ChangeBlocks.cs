using MSNTools.PersistentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSNTools
{
    public class CheckChangeBlocks
    {
        public static bool Exec(GameManager __instance, PlatformUserIdentifierAbs _persistentPlayerId, List<BlockChangeInfo> _blocksToChange)
        {
            try
            {
                if (__instance != null && _persistentPlayerId != null && _blocksToChange != null)
                {
                    ClientInfo cInfo = PersistentOperations.GetClientInfoFromPlatformUser(_persistentPlayerId);
                    if (cInfo != null)
                    {
                        EntityPlayer player = PersistentOperations.GetEntityPlayer(cInfo.entityId);
                        if (player != null)
                        {
                            int slot = player.inventory.holdingItemIdx;
                            ItemValue itemValue = cInfo.latestPlayerData.inventory[slot].itemValue;
                            World world = __instance.World;
                            for (int i = 0; i < _blocksToChange.Count; i++)
                            {
                                BlockChangeInfo newBlockInfo = _blocksToChange[i];//new block info
                                BlockValue oldBlockValue = world.GetBlock(newBlockInfo.pos);//old block value
                                Block oldBlock = oldBlockValue.Block;
                                if (newBlockInfo != null && newBlockInfo.bChangeBlockValue)//has new block value
                                {
                                    Block newBlock = newBlockInfo.blockValue.Block;
                                    if (oldBlockValue.type == BlockValue.Air.type)//old block was air
                                    {
                                        List<string> regionsReset = PersistentContainer.Instance.RegionsReset;
                                        PrefabInstance prefab = GameManager.Instance.World.GetPOIAtPosition(newBlockInfo.pos.ToVector3());
                                        if (newBlock is BlockSleepingBag)//placed a sleeping bag
                                        {
                                            if (prefab != null && prefab.prefab.HasQuestTag())
                                            {
                                                GameManager.Instance.World.SetBlockRPC(newBlockInfo.pos, BlockValue.Air);
                                                PersistentOperations.ReturnBlock(cInfo, newBlock.GetBlockName(), 1);
                                                return false;
                                            }
                                        }
                                        else if (newBlock is BlockLandClaim)//placed a land claim
                                        {
                                            if (prefab != null || Zones.IsInRegionReset(player))
                                            {
                                                GameManager.Instance.World.SetBlockRPC(newBlockInfo.pos, BlockValue.Air);
                                                PersistentOperations.ReturnBlock(cInfo, newBlock.GetBlockName(), 1);
                                                return false;
                                            }
                                        }
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in BlockChange.ProcessBlockChange: {e.Message}");
            }
            return true;
        }
    }
}