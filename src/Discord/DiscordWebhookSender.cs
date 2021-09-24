using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using SystemColor = System.Drawing.Color;

namespace MSNTools.Discord
{
    class ModEventsDiscordBehaviour
    {
        public static void GameStartDone()
        {
            if (DiscordWebhookSender.ServerInfosEnabled)
                DiscordWebhookSender.SendEmbedToWebHook(DiscordWebhookSender.EnumWebHookType.ServerInfos, DiscordWebhookSender.ServerOnlineColor, $"Serveur démarré !");
            if (DiscordWebhookSender.ChatEnabled)
                DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"*Serveur démarré !*");
        }

        public static void GameShutdown()
        {
            if (DiscordWebhookSender.ServerInfosEnabled)
                DiscordWebhookSender.SendEmbedToWebHook(DiscordWebhookSender.EnumWebHookType.ServerInfos, DiscordWebhookSender.ServerOfflineColor, $"Serveur éteint !");
            if (DiscordWebhookSender.ChatEnabled)
                DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"*Serveur éteint !*");
        }

        public static void PlayerSpawnedInWorld(ClientInfo _cInfo)
        {
            if (DiscordWebhookSender.PlayerInfosEnabled)
                DiscordWebhookSender.SendEmbedToWebHook(DiscordWebhookSender.EnumWebHookType.PlayerInfos, DiscordWebhookSender.PlayerConnectedColor, $"{_cInfo.playerName} vient de se connecter !");
            if (DiscordWebhookSender.ChatEnabled)
                DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"*{_cInfo.playerName} vient de se connecter !*");
        }

        public static void PlayerDisconnected(ClientInfo _cInfo)
        {
            if (DiscordWebhookSender.PlayerInfosEnabled)
                DiscordWebhookSender.SendEmbedToWebHook(DiscordWebhookSender.EnumWebHookType.PlayerInfos, DiscordWebhookSender.PlayerDisconnectedColor, $"{_cInfo.playerName} vient de se déconnecter !");
            if (DiscordWebhookSender.ChatEnabled)
                DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"*{_cInfo.playerName} vient de se déconnecter !*");
        }

        public static void ChatMessage(ClientInfo _cInfo, EChatType _type, string _msg)
        {
            if (DiscordWebhookSender.ChatEnabled)
            {
                if (_type.Equals(EChatType.Global))
                    DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"[{DateTime.UtcNow.ToLocalTime().ToString("dd-MM-yyyy - HH:mm:ss")}] [Global] **{_cInfo.playerName}** : {_msg}");
                else if (_type.Equals(EChatType.Friends))
                    DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"[Amis] **{_cInfo.playerName}** : {_msg}");
                else if (_type.Equals(EChatType.Party))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    EntityPlayer entityPlayer = PersistentOperations.GetEntityPlayer(_cInfo.playerId);

                    foreach (EntityPlayer member in entityPlayer.Party.MemberList)
                        stringBuilder.Append($"*{member.EntityName}*, ");

                    string members = stringBuilder.ToString().TrimEnd(',', ' ');
                    DiscordWebhookSender.SendChatMessageToWebhook(DiscordWebhookSender.EnumWebHookType.Chat, $"[Groupe] (Membres du groupe : {members})\n**{_cInfo.playerName}** : {_msg}");
                }
            }
        }
    }

    class DiscordWebhookSender
    {
        public static bool ChatEnabled, SanctionsEnabled, ServerInfosEnabled, PlayerInfosEnabled = false;
        public static string ChatWebHookUrl, SanctionsWebHookUrl, ServerInfosWebHookUrl, PlayerInfosWebHookUrl, FooterImageUrl = string.Empty;
        public static SystemColor PlayerConnectedColor, PlayerDisconnectedColor, SanctionsColor, ServerOnlineColor, ServerOfflineColor = SystemColor.Black;

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

        public static void SendSanctionToWebHook(ClientInfo _cInfo, string title, SystemColor color, string reason)
        {
            string steamID = _cInfo.playerId;
            string playerName = _cInfo.playerName;
            Vector3 playerPos = PersistentOperations.GetEntityPlayer(_cInfo.playerId).position;
            DiscordWebhook wbh = new DiscordWebhook();
            wbh.Url = SanctionsWebHookUrl;
            DiscordMessage msg = new DiscordMessage();
            msg.Embeds = new List<DiscordEmbed>();
            msg.Embeds.Add(new DiscordEmbed()
            {
                Title = $"{title}\n",
                Color = color,
                Footer = new EmbedFooter() { Text = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), IconUrl = FooterImageUrl },
                Fields = new List<EmbedField>() { new EmbedField() { Name = "Nom du joueur", Value = playerName, InLine = true }, new EmbedField() { Name = "Steam ID", Value = steamID, InLine = true }, new EmbedField() { Name = "Raison", Value = reason, InLine = false }, new EmbedField() { Name = "Position", Value = $"{playerPos}\n", InLine = false } }
            });
            wbh.Send(msg);
        }

        public static void SendEmbedToWebHook(EnumWebHookType webHookType, SystemColor color, string content)
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
            ServerInfos
        }
    }
}