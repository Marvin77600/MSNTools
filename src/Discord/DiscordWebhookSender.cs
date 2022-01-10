using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MSNTools.Discord
{
    class ModEventsDiscordBehaviour
    {
        public static void GameStartDone()
        {
            if (DiscordWebhookSender.ServerInfosEnabled)
                DiscordWebhookSender.SendEmbedToWebHook(DiscordWebhookSender.EnumWebHookType.ServerInfos, DiscordWebhookSender.ServerOnlineColor, $"Serveur démarré !");
            //if (DiscordWebhookSender.ChatEnabled)
                //DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"*Serveur démarré !*");
        }

        public static void GameShutdown()
        {
            if (DiscordWebhookSender.ServerInfosEnabled)
                DiscordWebhookSender.SendEmbedToWebHook(DiscordWebhookSender.EnumWebHookType.ServerInfos, DiscordWebhookSender.ServerOfflineColor, $"Serveur éteint !");
            //if (DiscordWebhookSender.ChatEnabled)
                //DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"*Serveur éteint !*");
        }

        public static void PlayerSpawnedInWorld(ClientInfo _cInfo)
        {
            if (_cInfo != null)
            {
                if (DiscordWebhookSender.PlayerInfosEnabled)
                    DiscordWebhookSender.SendEmbedToWebHook(DiscordWebhookSender.EnumWebHookType.PlayerInfos, DiscordWebhookSender.PlayerConnectedColor, $"{_cInfo.playerName} vient de se connecter !");
                //if (DiscordWebhookSender.ChatEnabled)
                //DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"*{_cInfo.playerName} vient de se connecter !*");
            }
        }

        public static void PlayerDisconnected(ClientInfo _cInfo)
        {
            if (_cInfo != null)
            {
                if (DiscordWebhookSender.PlayerInfosEnabled)
                    DiscordWebhookSender.SendEmbedToWebHook(DiscordWebhookSender.EnumWebHookType.PlayerInfos, DiscordWebhookSender.PlayerDisconnectedColor, $"{_cInfo.playerName} vient de se déconnecter !");
                //if (DiscordWebhookSender.ChatEnabled)
                //DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"*{_cInfo.playerName} vient de se déconnecter !*");
            }
        }

        public static void ChatMessage(ClientInfo _cInfo, EChatType _type, string _msg)
        {
            if (_cInfo != null)
            {
                if (DiscordWebhookSender.ChatEnabled)
                {
                    if (_type.Equals(EChatType.Global))
                        DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"[Global] **{_cInfo.playerName}** : {_msg}");
                    else if (_type.Equals(EChatType.Friends))
                        DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"[Amis] **{_cInfo.playerName}** : {_msg}");
                    else if (_type.Equals(EChatType.Party))
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        EntityPlayer entityPlayer = GameManager.Instance.World.Players.dict[_cInfo.entityId];

                        foreach (EntityPlayer member in entityPlayer.Party.MemberList)
                            stringBuilder.Append($"*{member.EntityName}*, ");

                        string members = stringBuilder.ToString().TrimEnd(',', ' ');
                        DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"[Groupe] (Membres du groupe : {members})\n**{_cInfo.playerName}** : {_msg}");
                    }
                }
            }
        }
    }

    class DiscordWebhookSender
    {
        public static bool ChatEnabled, SanctionsEnabled, ServerInfosEnabled, PlayerInfosEnabled, AlertsEnabled = false;
        public static string ChatWebHookUrl, SanctionsWebHookUrl, ServerInfosWebHookUrl, PlayerInfosWebHookUrl, AlertsWekHookUrl, FooterImageUrl = string.Empty;
        public static Color32 PlayerConnectedColor, PlayerDisconnectedColor, SanctionsColor, AlertsColor, ServerOnlineColor, ServerOfflineColor = Color.black;

        private static string GetWebHookUrl(EnumWebHookType webHookType)
        {
            string url = null;
            if (webHookType.Equals(EnumWebHookType.Chat))
                url = ChatWebHookUrl;
            else if (webHookType.Equals(EnumWebHookType.Sanction))
                url = SanctionsWebHookUrl;
            else if (webHookType.Equals(EnumWebHookType.PlayerInfos))
                url = PlayerInfosWebHookUrl;
            else if (webHookType.Equals(EnumWebHookType.ServerInfos))
                url = ServerInfosWebHookUrl;
            else if (webHookType.Equals(EnumWebHookType.Alerts))
                url = AlertsWekHookUrl;
            return url;
        }
        
        public static void SendChatMessageToWebhook(EnumWebHookType webHookType, string message)
        {
            DiscordWebhook wbh = new DiscordWebhook();
            wbh.Url = GetWebHookUrl(webHookType);
            DiscordMessage msg = new DiscordMessage();
            msg.Content = message;
            wbh.Send(msg);
        }

        public static void SendSanctionEmbedToWebHook(ClientInfo _cInfo, string title, string reason)
        {
            if (_cInfo != null)
            {
                string steamID = _cInfo.PlatformId.ToString().Substring(6);
                string eosID = _cInfo.CrossplatformId.ToString().Substring(4);
                string playerName = _cInfo.playerName;
                DiscordWebhook wbh = new DiscordWebhook();
                wbh.Url = GetWebHookUrl(EnumWebHookType.Sanction);
                DiscordMessage msg = new DiscordMessage();
                msg.Embeds = new List<DiscordEmbed>();
                msg.Embeds.Add(new DiscordEmbed()
                {
                    Title = $"{title}\n",
                    Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                    Fields = new List<EmbedField>()
                    {
                        new EmbedField() { Name = "Nom du joueur", Value = playerName, InLine = true },
                        new EmbedField() { Name = "Steam ID", Value = steamID, InLine = true },
                        new EmbedField() { Name = "EOS ID", Value = eosID, InLine = true },
                        new EmbedField() { Name = "Raison", Value = reason, InLine = false }
                    },
                    Color = SanctionsColor
                });
                wbh.Send(msg);
            }
        }

        public static void SendSanctionEmbedToWebHook(List<ItemStack> unauthorizedItems, ClientInfo _cInfo, string title, string reason)
        {
            if (_cInfo != null)
            {
                string steamID = _cInfo.PlatformId.ToString().Substring(6);
                string eosID = _cInfo.CrossplatformId.ToString().Substring(4);
                string playerName = _cInfo.playerName;
                DiscordWebhook wbh = new DiscordWebhook();
                wbh.Url = GetWebHookUrl(EnumWebHookType.Sanction);
                DiscordMessage msg = new DiscordMessage();
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < unauthorizedItems.Count; i++)
                {
                    if (i != unauthorizedItems.Count - 1)
                        stringBuilder.Append($"• {unauthorizedItems[i].itemValue.ItemClass.GetLocalizedItemName()}\n");
                    if (i == unauthorizedItems.Count - 1)
                        stringBuilder.Append($"• {unauthorizedItems[i].itemValue.ItemClass.GetLocalizedItemName()}");
                }
                msg.Embeds = new List<DiscordEmbed>();
                msg.Embeds.Add(new DiscordEmbed()
                {
                    Title = $"{title}\n",
                    Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                    Fields = new List<EmbedField>()
                    {
                        new EmbedField() { Name = "Nom du joueur", Value = playerName, InLine = true },
                        new EmbedField() { Name = "Steam ID", Value = steamID, InLine = true },
                        new EmbedField() { Name = "EOS ID", Value = eosID, InLine = true },
                        new EmbedField() { Name = "Raison", Value = reason, InLine = false },
                        new EmbedField() { Name = unauthorizedItems.Count == 1 ? "Item trouvé" : $"{unauthorizedItems.Count} items trouvés", Value = stringBuilder.ToString(), InLine = false },
                    },
                    Color = SanctionsColor
                });
                wbh.Send(msg);
            }
        }

        public static void SendSanctionEmbedToWebHook(Vector3 playerPos, ClientInfo _cInfo, string title, string reason)
        {
            if (_cInfo != null)
            {
                string steamID = _cInfo.PlatformId.ToString().Substring(6);
                string eosID = _cInfo.CrossplatformId.ToString().Substring(4);
                string playerName = _cInfo.playerName;
                DiscordWebhook wbh = new DiscordWebhook();
                wbh.Url = GetWebHookUrl(EnumWebHookType.Sanction);
                DiscordMessage msg = new DiscordMessage();
                msg.Embeds = new List<DiscordEmbed>();
                msg.Embeds.Add(new DiscordEmbed()
                {
                    Title = $"{title}\n",
                    Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                    Fields = new List<EmbedField>()
                    {
                        new EmbedField() { Name = "Nom du joueur", Value = playerName, InLine = true },
                        new EmbedField() { Name = "Steam ID", Value = steamID, InLine = true },
                        new EmbedField() { Name = "EOS ID", Value = eosID, InLine = true },
                        new EmbedField() { Name = "Raison", Value = reason, InLine = false },
                        new EmbedField() { Name = "Position", Value = $"{playerPos}\n", InLine = false }
                    },
                    Color = SanctionsColor
                });
                wbh.Send(msg);
            }
        }

        public static void SendAlertEmbedToWebHook(List<ItemStack> unauthorizedItems, TileEntitySecureLootContainer tileEntity)
        {
            ClientInfo _cInfo = PersistentOperations.GetClientInfoFromPlatformUser(tileEntity.GetOwner());
            if (_cInfo != null)
            {
                string steamID = _cInfo.PlatformId.ToString().Substring(6);
                string eosID = _cInfo.CrossplatformId.ToString().Substring(4);
                string playerName = _cInfo.playerName;
                Vector3i containerPos = tileEntity.ToWorldPos();
                DiscordWebhook wbh = new DiscordWebhook();
                wbh.Url = GetWebHookUrl(EnumWebHookType.Alerts);
                DiscordMessage msg = new DiscordMessage();
                msg.Embeds = new List<DiscordEmbed>();
                BlockValue container = GameManager.Instance.World.GetBlock(containerPos);
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < unauthorizedItems.Count; i++)
                {
                    if (i != unauthorizedItems.Count - 1)
                        stringBuilder.Append($"• {unauthorizedItems[i].itemValue.ItemClass.GetLocalizedItemName()}\n");
                    if (i == unauthorizedItems.Count - 1)
                        stringBuilder.Append($"• {unauthorizedItems[i].itemValue.ItemClass.GetLocalizedItemName()}");
                }
                msg.Embeds.Add(new DiscordEmbed()
                {
                    Title = $"Alerte items interdits dans {container.Block.GetLocalizedBlockName()}",
                    Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                    Fields = new List<EmbedField>()
                    {
                        new EmbedField() { Name = "Nom du propriétaire", Value = playerName, InLine = true },
                        new EmbedField() { Name = "Steam ID", Value = steamID, InLine = true },
                        new EmbedField() { Name = "EOS ID", Value = eosID, InLine = true },
                        new EmbedField() { Name = unauthorizedItems.Count == 1 ? "Item trouvé" : $"{unauthorizedItems.Count} items trouvés", Value = stringBuilder.ToString(), InLine = false },
                        new EmbedField() { Name = "Position du conteneur", Value = $"{containerPos}", InLine = false }
                    },
                    Color = AlertsColor
                });
                wbh.Send(msg);
            }
        }

        public static void SendAlertStartCollapseEmbedToWebHook(Vector3 pos, List<EntityPlayer> playersNearCollapse)
        {
            DiscordWebhook wbh = new DiscordWebhook();
            wbh.Url = GetWebHookUrl(EnumWebHookType.Alerts);
            DiscordMessage msg = new DiscordMessage();
            msg.Embeds = new List<DiscordEmbed>();
            if (playersNearCollapse == null || playersNearCollapse.Count == 0)
            {
                msg.Embeds.Add(new DiscordEmbed()
                {
                    Title = $"Alerte effondrement aux coordonnées {pos}",
                    Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                    Color = AlertsColor
                });
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i < playersNearCollapse.Count; i++)
                {
                    EntityPlayer player = playersNearCollapse[i];
                    if (i != playersNearCollapse.Count - 1)
                        stringBuilder.Append($"• {player.EntityName} ({(int)Vector3.Distance(pos, player.position)}m)\n");
                    if (i == playersNearCollapse.Count - 1)
                        stringBuilder.Append($"• {player.EntityName} ({(int)Vector3.Distance(pos, player.position)}m)");
                }

                msg.Embeds.Add(new DiscordEmbed()
                {
                    Title = $"Alerte effondrement aux coordonnées {pos}",
                    Fields = new List<EmbedField>()
                    {
                        new EmbedField() { Name = playersNearCollapse.Count == 1 ? "Joueur à proximité" : "Joueurs à proximité", Value = stringBuilder.ToString(), InLine = false }
                    },
                    Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                    Color = AlertsColor
                });
            }
            wbh.Send(msg);
        }

        public static void SendAlertFinishCollapseEmbedToWebHook(int fallingBlocksCount)
        {
            DiscordWebhook wbh = new DiscordWebhook();
            wbh.Url = GetWebHookUrl(EnumWebHookType.Alerts);
            DiscordMessage msg = new DiscordMessage();
            msg.Embeds = new List<DiscordEmbed>();
            msg.Embeds.Add(new DiscordEmbed()
            {
                Title = $"Effondrement terminé, un total de {fallingBlocksCount} entités de blocs ont été supprimés",
                Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                Color = AlertsColor
            });
            wbh.Send(msg);
        }

        public static void SendAlertFinishCollapseWithClosestPlayerEmbedToWebHook(int fallingBlocksCount, ClientInfo cInfo)
        {
            DiscordWebhook wbh = new DiscordWebhook();
            wbh.Url = GetWebHookUrl(EnumWebHookType.Alerts);
            DiscordMessage msg = new DiscordMessage();
            msg.Embeds = new List<DiscordEmbed>();
            msg.Embeds.Add(new DiscordEmbed()
            {
                Title = $"Effondrement terminé, un total de {fallingBlocksCount} entités de blocs ont été supprimés",
                Fields = new List<EmbedField> 
                {
                    new EmbedField() {  Name = "Joueur le plus proche", Value = $"{cInfo.playerName}", InLine = true }
                },
                Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                Color = AlertsColor
            });
            wbh.Send(msg);
        }

        public static void SendEmbedToWebHook(EnumWebHookType webHookType, Color32 color, string content)
        {
            DiscordWebhook wbh = new DiscordWebhook();
            wbh.Url = GetWebHookUrl(webHookType);
            DiscordMessage msg = new DiscordMessage();
            msg.Embeds = new List<DiscordEmbed>();
            msg.Embeds.Add(new DiscordEmbed()
            {
                Title = $"{content}",
                Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                Color = color
            });
            wbh.Send(msg);
        }
        
        public enum EnumWebHookType
        {
            Sanction,
            Chat,
            PlayerInfos,
            ServerInfos,
            Alerts
        }
    }
}