using System;
using System.Collections.Generic;

namespace MSNTools.Commands
{
    class ConsoleCmdDespawnEntity : ConsoleCmdAbstract
    {
        public override int DefaultPermissionLevel => 0;

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count != 1)
                {
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Wrong number of arguments, expected 1, found {_params.Count}.");
                }
                else
                {
                    ClientInfo cInfo = _senderInfo.RemoteClientInfo;
                    EntityAlive entity = null;
                    List<Entity> entities = GameManager.Instance.World.Entities.list;
                    foreach (Entity entity2 in entities)
                    {
                        if (entity2.entityId == int.Parse(_params[0]))
                            entity = (EntityAlive)entity2;
                    }
                    if (entity == null)
                        return;
                    if (entity is EntityPlayer)
                    {
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Cannot despawn a player.");
                        return;
                    }
                    if (entity is EntityDrone)
                    {
                        EntityDrone drone = (EntityDrone)entity;
                        ItemStack _itemStack = new ItemStack(drone.GetUpdatedItemValue(), 1);
                        EntityPlayer entityPlayer = GameManager.Instance.World.Players.dict[cInfo.entityId];
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
                        return;
                    }
                    if (entity == null)
                    {
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Playername or entity id not found.");
                    }
                    else
                    {
                        GameManager.Instance.World.RemoveEntity(entity.entityId, EnumRemoveEntityReason.Despawned);
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Entity {_params[0]} has despawned");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "despawnentity", "despawn" };

        public override string GetDescription() => "Despawn a given entity";

        public override string GetHelp() => "Despawn a given entity.\nUsage:\n   1. despawn <entity id>\n1. can be used to despawn any entity that can be despawed (zombies, vehicles, animals).";
    }
}