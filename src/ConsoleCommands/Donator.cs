using MSNTools.PersistentData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MSNTools.ConsoleCommands
{
    public class ConsoleCmdDonator : ConsoleCmdAbstract
    {
        public override int DefaultPermissionLevel => 0;

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                ClientInfo cInfo = _senderInfo.RemoteClientInfo;
                EntityPlayer player = null;
                List<EntityPlayer> players = GameManager.Instance.World.Players.list;
                foreach (EntityPlayer entity2 in players)
                {
                    if (entity2.entityId == int.Parse(_params[1]))
                        player = entity2;
                }
                if (player == null)
                {
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Wrong ID.");
                }
                else
                {
                    if (_params[0] == "add")
                    {
                        ClientInfo playerInfo = PersistentOperations.GetClientInfoFromEntityId(player.entityId);
                        PersistentContainer.Instance.Players[playerInfo.PlatformId.ToString()].IsDonator = true;
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"{playerInfo.playerName} est maintenant donateur.");

                        if (int.TryParse(_params[2], out int donation))
                        {
                            if (donation >= 10 && donation < 15)
                            {
                                GiveDonatorPack("donatorPack10", player, playerInfo);
                                SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Don du pack donateur 10€ à {playerInfo.playerName}.");
                            }
                            if (donation >= 15 && donation < 20)
                            {
                                GiveDonatorPack("donatorPack15", player, playerInfo);
                                SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Don du pack donateur 15€ à {playerInfo.playerName}.");
                            }
                            if (donation >= 20 && donation < 30)
                            {
                                GiveDonatorPack("donatorPack20", player, playerInfo);
                                SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Don du pack donateur 20€ à {playerInfo.playerName}.");
                            }
                            if (donation >= 30)
                            {
                                GiveDonatorPack("donatorPack30", player, playerInfo);
                                SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Don du pack donateur 30€ à {playerInfo.playerName}.");
                            }
                        }
                    }
                    if (_params[0] == "remove" || _params[0] == "del")
                    {
                        ClientInfo playerInfo = PersistentOperations.GetClientInfoFromEntityId(player.entityId);
                        PersistentContainer.Instance.Players[playerInfo.PlatformId.ToString()].IsDonator = false;
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"{playerInfo.playerName} n'est plus donateur.");
                    }
                    PersistentContainer.DataChange = true;
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        void GiveDonatorPack(string itemName, EntityPlayer player, ClientInfo cInfo)
        {
            ItemValue itemValue = new ItemValue(ItemClass.GetItem(itemName).type);
            EntityItem entityItem = (EntityItem)EntityFactory.CreateEntity(new EntityCreationData
            {
                entityClass = EntityClass.FromString("item"),
                id = EntityFactory.nextEntityID++,
                itemStack = new ItemStack(itemValue, 1),
                pos = player.position,
                rot = new Vector3(20f, 0f, 20f),
                lifetime = 60f,
                belongsPlayerId = cInfo.entityId
            });
            GameManager.Instance.World.SpawnEntityInWorld(entityItem);
            cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(entityItem.entityId, cInfo.entityId));
            GameManager.Instance.World.RemoveEntity(entityItem.entityId, EnumRemoveEntityReason.Despawned);
        }

        public override string[] GetCommands() => new string[] { "donator" };

        public override string GetDescription() => "...";
    }
}