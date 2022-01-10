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
                    Log.Out("CheckChangeBlocks.Exec");
                    ClientInfo cInfo = PersistentOperations.GetClientInfoFromPlatformUser(_persistentPlayerId);
                    if (cInfo != null)
                    {
                        Log.Out("1");
                        EntityPlayer player = PersistentOperations.GetEntityPlayer(cInfo.entityId);
                        if (player != null)
                        {
                            Log.Out("2");
                            int slot = player.inventory.holdingItemIdx;
                            Log.Out("2.1");
                            ItemValue itemValue = cInfo.latestPlayerData.inventory[slot].itemValue;
                            Log.Out("2.2");
                            Log.Out("3");
                            World world = __instance.World;
                            for (int i = 0; i < _blocksToChange.Count; i++)
                            {
                                BlockChangeInfo newBlockInfo = _blocksToChange[i];//new block info
                                Log.Out("4");
                                BlockValue oldBlockValue = world.GetBlock(newBlockInfo.pos);//old block value
                                Block oldBlock = oldBlockValue.Block;
                                if (newBlockInfo != null && newBlockInfo.bChangeBlockValue)//has new block value
                                {
                                    Log.Out("5");
                                    Block newBlock = newBlockInfo.blockValue.Block;
                                    if (oldBlockValue.type == BlockValue.Air.type)//old block was air
                                    {
                                        Log.Out("6");
                                        if (newBlock is BlockSleepingBag)//placed a sleeping bag
                                        {
                                            Log.Out("Block is SleepingBag");
                                            if (POIProtection.IsEnabled && POIProtection.Bed && world.IsPositionWithinPOI(newBlockInfo.pos.ToVector3(), 5))
                                            {
                                                Log.Out("Remove it");
                                                GameManager.Instance.World.SetBlockRPC(newBlockInfo.pos, BlockValue.Air);
                                                PersistentOperations.ReturnBlock(cInfo, newBlock.GetBlockName(), 1);
                                                return false;
                                            }
                                        }
                                        else if (newBlock is BlockLandClaim)//placed a land claim
                                        {
                                            Log.Out("Block is LandClaim");
                                            if (POIProtection.IsEnabled && POIProtection.Claim && world.IsPositionWithinPOI(newBlockInfo.pos.ToVector3(), 5))
                                            {
                                                Log.Out("Remove it");
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
                Log.Out($"{Config.ModPrefix} Error in BlockChange.ProcessBlockChange: {e.Message}");
            }
            return true;
        }
    }
}
