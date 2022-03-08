using MSNTools.Discord;
using MSNTools.PersistentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSNTools.ChatCommands
{
    public class ChatCommandTPToServerLocations
    {
        public static bool Exec(ClientInfo clientInfo, string msg)
        {
            EntityPlayer entityPlayer = GameManager.Instance.World.Players.dict[clientInfo.entityId];
            var serverLocations = PersistentContainer.Instance.ServerLocations;
            if (serverLocations.Count > 0)
            {
                if (msg.StartsWith(ChatCommandsHook.ChatCommandsPrefix) && ChatCommandsHook.ChatCommandsEnabled)
                {
                    foreach (var location in serverLocations)
                    {
                        if (msg.Contains(location.Key))
                        {
                            if (Bank.HasEnoughMoney(clientInfo, ChatCommandTP.TPCost))
                            {
                                SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"teleportplayer {entityPlayer.entityId} {string.Join(" ", location.Value)}", clientInfo);
                                if (DiscordWebhookSender.ServerInfosEnabled)
                                    DiscordWebhookSender.SendChatCommand(clientInfo, msg);
                                PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()].PlayerWallet -= ChatCommandTP.TPCost;
                                PersistentContainer.DataChange = true;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}