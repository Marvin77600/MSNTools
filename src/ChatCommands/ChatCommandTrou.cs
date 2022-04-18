using System;
using System.Collections.Generic;
using UnityEngine;

namespace MSNTools.ChatCommands
{
    public class ChatCommandTrou : ChatCommandAbstract
    {
        public override string Execute(List<string> _params, ClientInfo _clientInfo)
        {
            try
            {
                if (_clientInfo != null)
                {
                    EntityPlayer entityPlayer = GameManager.Instance.World.Players.dict[_clientInfo.entityId];
                    if (entityPlayer != null)
                    {
                        ItemValue itemValue = new ItemValue(ItemClass.GetItem("terrTopSoil").type);
                        EntityItem entityItem = (EntityItem)EntityFactory.CreateEntity(new EntityCreationData
                        {
                            entityClass = EntityClass.FromString("item"),
                            id = EntityFactory.nextEntityID++,
                            itemStack = new ItemStack(itemValue, 200),
                            pos = entityPlayer.position,
                            rot = new Vector3(20f, 0f, 20f),
                            lifetime = 60f,
                            belongsPlayerId = _clientInfo.entityId
                        });
                        GameManager.Instance.World.SpawnEntityInWorld(entityItem);
                        _clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(entityItem.entityId, _clientInfo.entityId));
                        GameManager.Instance.World.RemoveEntity(entityItem.entityId, EnumRemoveEntityReason.Despawned);
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
            return null;
        }

        public override string[] GetCommands() => new string[] { "trou" };
    }
}