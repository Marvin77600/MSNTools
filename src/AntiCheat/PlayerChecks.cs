using System;
using System.Collections.Generic;
using MSNTools.Discord;

namespace MSNTools
{
    class PlayerChecks
    {
        public static bool GodEnabled, SpectatorEnabled = false;
        public static int God_Admin_Level = 1, Spectator_Admin_Level = 1;

        public static void Exec()
        {
            try
            {
                if (GameManager.Instance.World != null && GameManager.Instance.World.Players.Count > 0 && GameManager.Instance.World.Players.dict.Count > 0)
                {
                    List<ClientInfo> _cInfoList = PersistentOperations.ClientList();
                    if (_cInfoList != null && _cInfoList.Count > 0)
                    {
                        foreach (ClientInfo _cInfo in _cInfoList)
                        {
                            if (_cInfo != null && _cInfo.playerId != null)
                            {
                                EntityPlayer _player = PersistentOperations.GetEntityPlayer(_cInfo.playerId);
                                if (_player != null)
                                {
                                    int _userPermissionLevel = GameManager.Instance.adminTools.GetUserPermissionLevel(_cInfo);

                                    if (SpectatorEnabled && _userPermissionLevel > Spectator_Admin_Level)
                                    {
                                        if (_player.IsSpectator)
                                        {
                                            SdtdConsole.Instance.ExecuteSync($"ban add {_cInfo.playerId} 10 years \"Caught in spectator mode\"", null);
                                            Log.Warning($"{Config.ModPrefix} Detected \"{_cInfo.playerName}\", Steam Id {_cInfo.playerId}, using spectator mode @ {(int)_player.position.x} {(int)_player.position.y} {(int)_player.position.z}");
                                            if (DiscordWebhookSender.SanctionsEnabled)
                                                DiscordWebhookSender.SendSanctionToWebHook(_cInfo, $"Ban de {_cInfo.playerName}", DiscordWebhookSender.SanctionsColor, "Détecté en mode spectateur");
                                            continue;
                                        }
                                    }
                                    if (GodEnabled && _userPermissionLevel > God_Admin_Level)
                                    {
                                        if (_player.Buffs.HasBuff("god"))
                                        {
                                            SdtdConsole.Instance.ExecuteSync($"ban add {_cInfo.playerId} 10 years \"Caught in god mode\"", null);
                                            Log.Warning($"{Config.ModPrefix} Detected \"{_cInfo.playerName}\", Steam id {_cInfo.playerId}, using god mode @ {(int)_player.position.x} {(int)_player.position.y} {(int)_player.position.z}");
                                            if (DiscordWebhookSender.SanctionsEnabled)
                                                DiscordWebhookSender.SendSanctionToWebHook(_cInfo, $"Ban de {_cInfo.playerName}", DiscordWebhookSender.SanctionsColor, "Détecté en god mode");
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out($"{Config.ModPrefix} Error in PlayerChecks.Exec: {e.Message}");
            }
        }
    }
}
