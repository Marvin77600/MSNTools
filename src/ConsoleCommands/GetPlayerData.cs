using System;
using System.Collections.Generic;
using MSNTools.PersistentData;
using MSNTools.Discord;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSNTools.ConsoleCommands
{
    public class GetPlayerData : ConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count != 1)
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Wrong number of arguments, expected 1, found {_params.Count}.");
                else
                {
                    if (_params[0].Length == 23)
                    {
                        PersistentPlayer player = PersistentContainer.Instance.Players[_params[0].ToString()];
                        if (player != null)
                            DiscordWebhookSender.SendPlayerData(player);
                        else
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Player is not connected or doesn't exists.");
                    }
                    else
                    {
                        var players = GameManager.Instance.World.Players.list;
                        EntityPlayer entityPlayer = null;
                        foreach (EntityPlayer entity2 in players)
                        {
                            if (entity2.entityId == int.Parse(_params[0]))
                                entityPlayer = entity2;
                        }
                        if (entityPlayer != null)
                        {
                            ClientInfo cInfo = PersistentOperations.GetClientInfoFromEntityId(entityPlayer.entityId);
                            PersistentPlayer player = PersistentContainer.Instance.Players[cInfo.PlatformId.ToString()];

                            if (player != null)
                                DiscordWebhookSender.SendPlayerData(player);
                            else
                                SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Player is not connected or doesn't exists.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "getplayerdata", "gpd" };

        public override string GetDescription() => "...";
    }
}
