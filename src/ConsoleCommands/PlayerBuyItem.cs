using MSNTools.Discord;
using MSNTools.PersistentData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MSNTools.ConsoleCommands
{
    public class PlayerBuyItem : MSNConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count != 5)
                {
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Wrong number of arguments, expected 5, found {_params.Count}.");
                }
                else
                {
                    string steamID = _params[0];
                    string itemName = _params[1];
                    int quality = StringParsers.ParseSInt32(_params[2]);
                    int count = StringParsers.ParseSInt32(_params[3]);
                    int price = StringParsers.ParseSInt32(_params[4]);
                    var clientsList = PersistentOperations.ClientList();

                    int playerId = -1;
                    ClientInfo clientInfo = null;
                    foreach (var client in clientsList)
                    {
                        if (client.PlatformId.ToString() == steamID)
                        {
                            playerId = client.entityId;
                            clientInfo = client;
                            break;
                        }
                    }

                    EntityPlayer entityPlayer = GameManager.Instance.World.Players.dict[playerId];
                    if (entityPlayer != null)
                    {
                        MSNUtils.Log("Player not null");
                        ItemValue itemValue = new ItemValue(ItemClass.GetItem(itemName).type, quality, quality);
                        EntityItem entityItem = (EntityItem)EntityFactory.CreateEntity(new EntityCreationData
                        {
                            entityClass = EntityClass.FromString("item"),
                            id = EntityFactory.nextEntityID++,
                            itemStack = new ItemStack(itemValue, count),
                            pos = entityPlayer.position,
                            rot = new Vector3(20f, 0f, 20f),
                            lifetime = 60f,
                            belongsPlayerId = clientInfo.entityId
                        });
                        PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()].PlayerWallet -= price;
                        PersistentContainer.DataChange = true;
                        GameManager.Instance.World.SpawnEntityInWorld(entityItem);
                        clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(entityItem.entityId, clientInfo.entityId));
                        GameManager.Instance.World.RemoveEntity(entityItem.entityId, EnumRemoveEntityReason.Despawned);
                        DiscordWebhookSender.SendNotifShop(clientInfo, _params.ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "pbi" };

        public override string GetDescription()
        {
            throw new NotImplementedException();
        }
    }
}
