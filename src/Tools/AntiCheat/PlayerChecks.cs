using MSNTools.Discord;
using System;
using System.Collections.Generic;

namespace MSNTools
{
    class PlayerChecks
    {
        public static bool GodEnabled, SpectatorEnabled = false;
        public static int God_Admin_Level = 1, Spectator_Admin_Level = 1;

        /// <summary>
        /// Check si des joueurs sont en godmode ou en mode spectateur, si oui ban direct du joueur.
        /// </summary>
        public static void Exec()
        {
            try
            {
                if (GameManager.Instance.World != null && GameManager.Instance.World.Players.Count > 0 && GameManager.Instance.World.Players.dict.Count > 0)
                {
                    List<EntityPlayer> _playersList = GameManager.Instance.World.Players.list;

                    if (_playersList != null && _playersList.Count > 0)
                    {
                        for (int index = 0; index < _playersList.Count; index++)
                        {
                            EntityPlayer player = _playersList[index];
                            ClientInfo _cInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(player.entityId);
                            if (player != null && _cInfo != null)
                            {
                                string steamID = _cInfo.PlatformId.CombinedString.Substring(6, _cInfo.PlatformId.ToString().Length - 6);
                                if (player != null)
                                {
                                    int _userPermissionLevel = GameManager.Instance.adminTools.GetUserPermissionLevel(_cInfo);

                                    if (SpectatorEnabled && _userPermissionLevel > Spectator_Admin_Level)
                                    {
                                        if (player.IsSpectator)
                                        {
                                            SdtdConsole.Instance.ExecuteSync($"ban add {_cInfo.entityId} 10 years \"Caught in spectator mode\"", null);
                                            MSNUtils.LogWarning($"Detected \"{_cInfo.playerName}\", Steam Id {steamID}, using spectator mode @ {(int)player.position.x} {(int)player.position.y} {(int)player.position.z}");
                                            if (DiscordWebhookSender.SanctionsEnabled)
                                                DiscordWebhookSender.SendSanctionEmbedToWebHook(player.position, _cInfo, $"Ban de {_cInfo.playerName}", "Détecté en mode spectateur");
                                            continue;
                                        }
                                    }
                                    if (GodEnabled && _userPermissionLevel > God_Admin_Level)
                                    {
                                        if (player.Buffs.HasBuff("god"))
                                        {
                                            SdtdConsole.Instance.ExecuteSync($"ban add {_cInfo.entityId} 10 years \"Caught in god mode\"", null);
                                            MSNUtils.LogWarning($"Detected \"{_cInfo.playerName}\", Steam id {steamID}, using god mode @ {(int)player.position.x} {(int)player.position.y} {(int)player.position.z}");
                                            if (DiscordWebhookSender.SanctionsEnabled)
                                                DiscordWebhookSender.SendSanctionEmbedToWebHook(player.position, _cInfo, $"Ban de {_cInfo.playerName}", "Détecté en god mode");
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
                MSNUtils.LogError($"Error in PlayerChecks.Exec: {e.Message}");
            }
        }
    }
}
