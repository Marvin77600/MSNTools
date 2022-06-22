using System;
using UnityEngine;

namespace MSNTools
{
    public class KickBloodTower
    {
        public static string PrefabName = string.Empty;
        public static bool IsEnabled = false;
        public static Vector3i TPPosition = new Vector3i();

        public static void Exec()
        {
            try
            {
                if (GameManager.Instance.World != null && GameManager.Instance.World.Players.Count > 0)
                {
                    var players = GameManager.Instance.World.Players.list;

                    foreach (var player in players)
                    {
                        PrefabInstance prefab = GameManager.Instance.World.GetPOIAtPosition(player.position);
                        MSNUtils.LogWarning(prefab.name);
                        if (prefab != null && prefab.name.ContainsCaseInsensitive(PrefabName) && player.Progression.Level > 30)
                        {
                            ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(player.entityId);
                            int _userPermissionLevel = GameManager.Instance.adminTools.GetUserPermissionLevel(clientInfo);
                            if (_userPermissionLevel > 1) TeleportOutPrefab(player);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in KickBloodTower.Exec: {e.Message}");
            }
        }

        public static void TeleportOutPrefab(EntityPlayer player)
        {
            if (player.Buffs.HasBuff("god"))
                return;
            player.Teleport(TPPosition);
        }
    }
}