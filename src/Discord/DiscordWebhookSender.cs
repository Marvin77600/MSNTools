using MSNTools.PersistentData;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MSNTools.Discord
{
    class ModEventsDiscordBehaviour
    {
        private static MSNLocalization.Language ServerLanguage = PersistentContainer.Instance.ServerLanguage;

        public static void GameStartDone()
        {
            if (DiscordWebhookSender.ServerInfosEnabled)
                DiscordWebhookSender.SendEmbedToWebHook(DiscordWebhookSender.EnumWebHookType.ServerInfos, DiscordWebhookSender.ServerOnlineColor, MSNLocalization.Get("discordServerStarted", ServerLanguage));
            //if (DiscordWebhookSender.ChatEnabled)
                //DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"*Serveur démarré !*");
        }

        public static void GameShutdown()
        {
            if (DiscordWebhookSender.ServerInfosEnabled)
                DiscordWebhookSender.SendEmbedToWebHook(DiscordWebhookSender.EnumWebHookType.ServerInfos, DiscordWebhookSender.ServerOfflineColor, MSNLocalization.Get("discordServerShutdown", ServerLanguage));
            //if (DiscordWebhookSender.ChatEnabled)
                //DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"*Serveur éteint !*");
        }

        public static void PlayerSpawnedInWorld(ClientInfo _cInfo)
        {
            if (_cInfo != null)
            {
                if (DiscordWebhookSender.PlayerInfosEnabled)
                    DiscordWebhookSender.SendEmbedToWebHook(DiscordWebhookSender.EnumWebHookType.PlayerInfos, DiscordWebhookSender.PlayerConnectedColor, MSNLocalization.Get("discordPlayerSpawnedInWorld", ServerLanguage, _cInfo.playerName));
                //if (DiscordWebhookSender.ChatEnabled)
                //DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"*{_cInfo.playerName} vient de se connecter !*");
            }
        }

        public static void PlayerDisconnected(ClientInfo _cInfo)
        {
            if (_cInfo != null)
            {
                if (DiscordWebhookSender.PlayerInfosEnabled)
                    DiscordWebhookSender.SendEmbedToWebHook(DiscordWebhookSender.EnumWebHookType.PlayerInfos, DiscordWebhookSender.PlayerDisconnectedColor, MSNLocalization.Get("discordPlayerDisconnected", ServerLanguage, _cInfo.playerName));
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
                    if (_type.Equals(EChatType.Global) && (DiscordWebhookSender.ChatType == DiscordWebhookSender.EnumChatType.Global || DiscordWebhookSender.ChatType ==  DiscordWebhookSender.EnumChatType.All))
                        DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"[Global] **{_cInfo.playerName}** : {_msg}");
                    else if (_type.Equals(EChatType.Friends) && (DiscordWebhookSender.ChatType == DiscordWebhookSender.EnumChatType.Friends || DiscordWebhookSender.ChatType == DiscordWebhookSender.EnumChatType.All))
                        DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"[{MSNLocalization.Get("discordFriends", ServerLanguage)}] **{_cInfo.playerName}** : {_msg}");
                    else if (_type.Equals(EChatType.Party) && (DiscordWebhookSender.ChatType == DiscordWebhookSender.EnumChatType.Party || DiscordWebhookSender.ChatType == DiscordWebhookSender.EnumChatType.All))
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        EntityPlayer entityPlayer = GameManager.Instance.World.Players.dict[_cInfo.entityId];

                        foreach (EntityPlayer member in entityPlayer.Party.MemberList)
                            stringBuilder.Append($"*{member.EntityName}*, ");

                        string members = stringBuilder.ToString().TrimEnd(',', ' ');
                        DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, MSNLocalization.Get("discordParty", ServerLanguage, members, _cInfo.playerName, _msg));
                    }
                }
            }
        }
    }

    class DiscordWebhookSender
    {
        private static MSNLocalization.Language ServerLanguage = PersistentContainer.Instance.ServerLanguage;

        public static bool ChatEnabled, SanctionsEnabled, ServerInfosEnabled, PlayerInfosEnabled, AlertsEnabled, BloodMoonAlertsEnabled = false;
        public static string ChatWebHookUrl, SanctionsWebHookUrl, ServerInfosWebHookUrl, PlayerInfosWebHookUrl, AlertsWekHookUrl, BloodMoonWebHookUrl, FooterImageUrl, BloodMoonAlertImageUrl = string.Empty;
        public static Color32 PlayerConnectedColor, PlayerDisconnectedColor, SanctionsColor, AlertsColor, ServerOnlineColor, ServerOfflineColor = Color.black;
        public static EnumChatType ChatType = new EnumChatType();

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
            else if (webHookType.Equals(EnumWebHookType.BloodMoon))
                url = BloodMoonWebHookUrl;
            return url;
        }

        public static void SendChatCommand(ClientInfo _cInfo, string cmd)
        {
            if (_cInfo != null && cmd != string.Empty)
            {
                string playerName = _cInfo.playerName;
                DiscordWebhook wbh = new DiscordWebhook();
                wbh.Url = GetWebHookUrl(EnumWebHookType.PlayerInfos);
                DiscordMessage msg = new DiscordMessage();
                msg.Embeds = new List<DiscordEmbed>();
                msg.Embeds.Add(new DiscordEmbed()
                {
                    Title = MSNLocalization.Get("discordSendChatCommandTitle", ServerLanguage, playerName, cmd),
                    Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                });
                wbh.Send(msg);
            }
        }

        public static void SendBloodMoonAlert()
        {
            DiscordWebhook wbh = new DiscordWebhook();
            wbh.Url = GetWebHookUrl(EnumWebHookType.BloodMoon);
            DiscordMessage msg = new DiscordMessage();
            msg.Embeds = new List<DiscordEmbed>();
            msg.Embeds.Add(new DiscordEmbed()
            {
                Title = MSNLocalization.Get("discordSendBloodMoonAlert", ServerLanguage, GamePrefs.GetString(EnumGamePrefs.ServerName)),
                Image = new EmbedMedia()
                {
                    Url = BloodMoonAlertImageUrl
                },
                Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                Color = Color.red
            });
            wbh.Send(msg);
        }

        public static void SendBloodMoonStart()
        {
            DiscordWebhook wbh = new DiscordWebhook();
            wbh.Url = GetWebHookUrl(EnumWebHookType.BloodMoon);
            DiscordMessage msg = new DiscordMessage();
            msg.Embeds = new List<DiscordEmbed>();
            msg.Embeds.Add(new DiscordEmbed()
            {
                Title = MSNLocalization.Get("discordSendBloodMoonStart", ServerLanguage, GamePrefs.GetString(EnumGamePrefs.ServerName)),
                Image = new EmbedMedia()
                {
                    Url = BloodMoonAlertImageUrl
                },
                Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                Color = Color.red
            });
            wbh.Send(msg);
        }

        public static void SendBloodMoonEnd()
        {
            DiscordWebhook wbh = new DiscordWebhook();
            wbh.Url = GetWebHookUrl(EnumWebHookType.BloodMoon);
            DiscordMessage msg = new DiscordMessage();
            msg.Embeds = new List<DiscordEmbed>();
            msg.Embeds.Add(new DiscordEmbed()
            {
                Title = MSNLocalization.Get("discordSendBloodMoonEnd", ServerLanguage, GamePrefs.GetString(EnumGamePrefs.ServerName)),
                Image = new EmbedMedia()
                {
                    Url = BloodMoonAlertImageUrl
                },
                Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                Color = Color.red
            });
            wbh.Send(msg);
        }

        public static void SendChatMessageToWebhook(EnumWebHookType webHookType, string message)
        {
            DiscordWebhook wbh = new DiscordWebhook();
            wbh.Url = GetWebHookUrl(webHookType);
            DiscordMessage msg = new DiscordMessage();
            msg.Content = message;
            wbh.Send(msg);
        }

        public static void SendPlayerData(PersistentPlayer persistentPlayer)
        {
            if (persistentPlayer != null)
            {
                string playerName = persistentPlayer.PlayerName;
                DiscordWebhook wbh = new DiscordWebhook();
                wbh.Url = GetWebHookUrl(EnumWebHookType.PlayerInfos);
                DiscordMessage msg = new DiscordMessage();
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var tpPosition in persistentPlayer.TPPositions)
                {
                    stringBuilder.Append($"• {tpPosition.Key} - {tpPosition.Value}\n");
                }
                stringBuilder.ToString().TrimEnd('\n');
                msg.Embeds = new List<DiscordEmbed>();
                msg.Embeds.Add(new DiscordEmbed()
                {
                    Title = $"Données du joueur {playerName}",
                    Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                    Fields = new List<EmbedField>()
                    {
                        new EmbedField() { Name = "Steam ID", Value = persistentPlayer.platformIdentifierString.Substring(6), InLine = false },
                        new EmbedField() { Name = MSNLocalization.Get("languageKey", ServerLanguage), Value = persistentPlayer.Language.ToString(), InLine = false },
                        new EmbedField() { Name = MSNLocalization.Get("walletKey", ServerLanguage), Value = persistentPlayer.PlayerWallet.ToString(), InLine = false },
                        new EmbedField() { Name = MSNLocalization.Get("donatorKey", ServerLanguage), Value = persistentPlayer.IsDonator ? MSNLocalization.Get("yesKey", ServerLanguage) : MSNLocalization.Get("noKey", ServerLanguage), InLine = false },
                        new EmbedField() { Name = "TP perso", Value = stringBuilder.ToString(), InLine = false },
                    },
                    Color = Color.grey
                });
                wbh.Send(msg);
            }
        }

        public static void SendResetRegionMessage()
        {
            DiscordWebhook wbh = new DiscordWebhook();
            wbh.Url = GetWebHookUrl(EnumWebHookType.ServerInfos);
            DiscordMessage msg = new DiscordMessage();
            msg.Embeds = new List<DiscordEmbed>();
            msg.Embeds.Add(new DiscordEmbed()
            {
                Title = MSNLocalization.Get("discordResetRegionMessageTitle", ServerLanguage, GamePrefs.GetString(EnumGamePrefs.ServerName)),
                Fields = new List<EmbedField>()
                {
                    new EmbedField() { Name = MSNLocalization.Get("discordResetRegionNext", ServerLanguage), Value = $"{PersistentContainer.Instance.TimeRegionFiles.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss")}", InLine = false }
                },
                Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                Color = Color.grey
            });
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
                        new EmbedField() { Name = MSNLocalization.Get("playerNameKey", ServerLanguage), Value = playerName, InLine = true },
                        new EmbedField() { Name = "Steam ID", Value = steamID, InLine = true },
                        new EmbedField() { Name = "EOS ID", Value = eosID, InLine = true },
                        new EmbedField() { Name = MSNLocalization.Get("reasonKey", ServerLanguage), Value = reason, InLine = false }
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
                        stringBuilder.Append($"• {unauthorizedItems[i].itemValue.ItemClass.GetLocalizedItemName()} ({unauthorizedItems[i].itemValue.ItemClass.Name})\n");
                    if (i == unauthorizedItems.Count - 1)
                        stringBuilder.Append($"• {unauthorizedItems[i].itemValue.ItemClass.GetLocalizedItemName()} ({unauthorizedItems[i].itemValue.ItemClass.Name})");
                }
                msg.Embeds = new List<DiscordEmbed>();
                msg.Embeds.Add(new DiscordEmbed()
                {
                    Title = $"{title}\n",
                    Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                    Fields = new List<EmbedField>()
                    {
                        new EmbedField() { Name = MSNLocalization.Get("playerNameKey", ServerLanguage), Value = playerName, InLine = true },
                        new EmbedField() { Name = "Steam ID", Value = steamID, InLine = true },
                        new EmbedField() { Name = "EOS ID", Value = eosID, InLine = true },
                        new EmbedField() { Name = MSNLocalization.Get("reasonKey", ServerLanguage), Value = reason, InLine = false },
                        new EmbedField() { Name = unauthorizedItems.Count == 1 ? MSNLocalization.Get("itemFoundKey", ServerLanguage) : MSNLocalization.Get("itemsFoundKey", ServerLanguage, unauthorizedItems.Count), Value = stringBuilder.ToString(), InLine = false },
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
                        new EmbedField() { Name = MSNLocalization.Get("playerNameKey", ServerLanguage), Value = playerName, InLine = true },
                        new EmbedField() { Name = "Steam ID", Value = steamID, InLine = true },
                        new EmbedField() { Name = "EOS ID", Value = eosID, InLine = true },
                        new EmbedField() { Name = MSNLocalization.Get("reasonKey", ServerLanguage), Value = reason, InLine = false },
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
                    Title = MSNLocalization.Get("discordSendAlertTitle", ServerLanguage, container.Block.GetLocalizedBlockName()),
                    Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                    Fields = new List<EmbedField>()
                    {
                        new EmbedField() { Name = MSNLocalization.Get("ownerNameKey", ServerLanguage), Value = playerName, InLine = true },
                        new EmbedField() { Name = "Steam ID", Value = steamID, InLine = true },
                        new EmbedField() { Name = "EOS ID", Value = eosID, InLine = true },
                        new EmbedField() { Name = unauthorizedItems.Count == 1 ? MSNLocalization.Get("itemFoundKey", ServerLanguage) : MSNLocalization.Get("itemsFoundKey", ServerLanguage, unauthorizedItems.Count), Value = stringBuilder.ToString(), InLine = false },
                        new EmbedField() { Name = MSNLocalization.Get("containerPositionKey", ServerLanguage), Value = $"{containerPos}", InLine = false }
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
                    Title = MSNLocalization.Get("collapseStartedKey", ServerLanguage, pos),
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
                    Title = MSNLocalization.Get("collapseStartedKey", ServerLanguage, pos),
                    Fields = new List<EmbedField>()
                    {
                        new EmbedField() { Name = playersNearCollapse.Count == 1 ? MSNLocalization.Get("playerNearKey", ServerLanguage) : MSNLocalization.Get("playersNearKey", ServerLanguage), Value = stringBuilder.ToString(), InLine = false }
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
                Title = MSNLocalization.Get("collapseFinishKey", ServerLanguage, fallingBlocksCount),
                Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                Color = AlertsColor
            });
            wbh.Send(msg);
        }

        public static void SendAlertPlayerKilledAtTrader(ClientInfo killerClientInfo, ClientInfo targetClientInfo)
        {
            if (killerClientInfo != null && targetClientInfo != null)
            {
                string killerSteamID = killerClientInfo.PlatformId.ToString().Substring(6);
                string targetSteamID = targetClientInfo.PlatformId.ToString().Substring(6);
                DiscordWebhook wbh = new DiscordWebhook();
                wbh.Url = GetWebHookUrl(EnumWebHookType.Alerts);
                DiscordMessage msg = new DiscordMessage();
                msg.Embeds = new List<DiscordEmbed>();
                msg.Embeds.Add(new DiscordEmbed()
                {
                    Title = MSNLocalization.Get("playerKilledAtTrader", ServerLanguage, killerClientInfo.playerName, killerSteamID, targetClientInfo.playerName, targetSteamID),
                    Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                    Color = AlertsColor
                });
                wbh.Send(msg);
            }
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
            Alerts,
            BloodMoon
        }

        public enum EnumChatType
        {
            Global,
            Friends,
            Party,
            All
        }
    }
}