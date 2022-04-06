using HarmonyLib;
using MSNTools.Discord;
using System;
using System.Collections.Generic;

namespace MSNTools.Harmony
{
    public class H_ConsoleCmdBan
    {
        [HarmonyPatch(typeof(ConsoleCmdBan), "Execute")]
        [HarmonyPatch(new Type[] { typeof(List<string>), typeof(CommandSenderInfo) })]
        public class H_ConsoleCmdBan_Execute
        {
            private static void Postfix(List<string> _params, CommandSenderInfo _senderInfo)
            {
                if (DiscordWebhookSender.SanctionsEnabled)
                {
                    if (_params.Count >= 1)
                    {
                        if (_params[0].EqualsCaseInsensitive("add") && !_params[4].EqualsCaseInsensitive("Caught with an unauthorized item:") && !_params[4].EqualsCaseInsensitive("Caught in spectator mode") && !_params[4].EqualsCaseInsensitive("Caught in god mode"))
                        {
                            Function(_params, _senderInfo);
                        }
                    }
                }
            }

            private static void Function(List<string> _params, CommandSenderInfo _senderInfo)
            {
                try
                {
                    if (_params.Count < 4)
                    {
                        return;
                    }
                    else
                    {
                        ClientInfo _senderClientInfo = _senderInfo.RemoteClientInfo;
                        string _senderClientName = null;
                        if (_senderClientInfo != null)
                            _senderClientName = _senderClientInfo.playerName;
                        string _name = null;
                        if (_params.Count > 5)
                            _name = _params[5];
                        PlatformUserIdentifierAbs _id;
                        ClientInfo playerBanned;
                        if (ConsoleHelper.ParseParamPartialNameOrId(_params[1], out _id, out playerBanned) != 1)
                            return;
                        int result = int.Parse(_params[2]);
                        if (playerBanned != null)
                        {
                            if (_name == null)
                                _name = playerBanned.playerName;
                        }
                        else
                        {
                            string _banTime = null;
                            if (_params[3].EqualsCaseInsensitive("min") || _params[3].EqualsCaseInsensitive("minute") || _params[3].EqualsCaseInsensitive("minutes"))
                                _banTime = result == 1 ? "minute" : "minutes";
                            else if (_params[3].EqualsCaseInsensitive("h") || _params[3].EqualsCaseInsensitive("hour") || _params[3].EqualsCaseInsensitive("hours"))
                                _banTime = result == 1 ? "heure" : "heures";
                            else if (_params[3].EqualsCaseInsensitive("d") || _params[3].EqualsCaseInsensitive("day") || _params[3].EqualsCaseInsensitive("days"))
                                _banTime = result == 1 ? "jour" : "jours";
                            else if (_params[3].EqualsCaseInsensitive("w") || _params[3].EqualsCaseInsensitive("week") || _params[3].EqualsCaseInsensitive("weeks"))
                                _banTime = result == 1 ? "semaine" : "semaines";
                            else if (_params[3].EqualsCaseInsensitive("month") || _params[3].EqualsCaseInsensitive("months"))
                                _banTime = "mois";
                            else if (_params[3].EqualsCaseInsensitive("y") || _params[3].EqualsCaseInsensitive("yr") || _params[3].EqualsCaseInsensitive("year") || _params[3].EqualsCaseInsensitive("years"))
                                _banTime = result == 1 ? "année" : "années";
                            else
                                return;
                            string reason = string.Empty;
                            if (_params.Count > 4)
                                reason = _params[4];
                            if (_senderClientName != null)
                                DiscordWebhookSender.SendSanctionEmbedToWebHook(playerBanned, $"Ban de {_name} par {_senderClientName}", reason == string.Empty ? $"{_name} banni pour une durée de {result} {_banTime}" : $"{_name} banni pour une durée de {result} {_banTime}\nRaison : {reason}");
                            if (_senderClientName == null)
                                DiscordWebhookSender.SendSanctionEmbedToWebHook(playerBanned, $"Ban de {_name}", reason == string.Empty ? $"{_name} banni pour une durée de {result} {_banTime}" : $"{_name} banni pour une durée de {result} {_banTime}\nRaison : {reason}");
                        }
                    }
                }
                catch (Exception e)
                {
                    MSNUtils.LogError($"Error in PatchAddWebhookConsoleCmdBan.Function: {e.Message}");
                }
            }
        }
    }
}