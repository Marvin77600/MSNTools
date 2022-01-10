using System;
using System.Collections.Generic;

namespace MSNTools.ChatCommands
{
    public class ChatCommandGetMyDrone : ChatCommandAbstract
    {
        public override void Execute(List<string> _params, ClientInfo _clientInfo)
        {
            try
            {
                if (_clientInfo != null)
                {
                    EntityPlayer entityPlayer = GameManager.Instance.World.Players.dict[_clientInfo.entityId];

                    List<Entity> entities = GameManager.Instance.World.Entities.list;

                    foreach (Entity entity in entities)
                    {
                        if (entity is EntityDrone)
                        {
                            EntityDrone drone = (EntityDrone)entity;
                            ItemStack _itemStack = new ItemStack(drone.GetUpdatedItemValue(), 1);
                            if (drone.GetOwner().ToString() == _clientInfo.CrossplatformId.ToString())
                            {
                                if (entityPlayer.inventory.CanTakeItem(_itemStack) || entityPlayer.bag.CanTakeItem(_itemStack))
                                {
                                    drone.PlaySound(entityPlayer, "drone_take", true, true);
                                    drone.IsEntityUpdatedInUnloadedChunk = false;
                                    drone.bWillRespawn = false;
                                    GameManager.Instance.CollectEntityServer(drone.entityId, entityPlayer.entityId);
                                    if (entityPlayer.Buffs.HasBuff("buffJunkDroneSupportEffect"))
                                        entityPlayer.Buffs.RemoveBuff("buffJunkDroneSupportEffect");
                                    if (entityPlayer.Party == null)
                                        return;
                                    for (int index = 0; index < entityPlayer.Party.MemberList.Count; ++index)
                                    {
                                        if (!entityPlayer || !entityPlayer.Buffs.HasBuff("buffJunkDroneSupportEffect"))
                                            return;
                                        entityPlayer.Buffs.RemoveBuff("buffJunkDroneSupportEffect");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "getmydrone", "gmd" };
    }
}