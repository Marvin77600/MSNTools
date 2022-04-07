using MSNTools.PersistentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

namespace MSNTools
{
    public class Zones
    {
        public static string BuffResetZone = "";
        public static List<int[]> ProtectedList = new List<int[]>();
        public static List<string> AddProtection = new List<string>();
        public static List<string> RemoveProtection = new List<string>();
        public static bool IsEnabled = false;

        /// <summary>
        /// Ajoute le <see cref="BuffResetZone"/> aux joueurs se trouvant dans des reset regions.
        /// </summary>
        public static void Exec()
        {
            try
            {
                if (GameManager.Instance.World != null && GameManager.Instance.World.Players.Count > 0 && GameManager.Instance.World.Players.dict.Count > 0)
                {
                    List<EntityPlayer> players = GameManager.Instance.World.Players.list;

                    for (int i = 0; i < players.Count; i++)
                    {
                        EntityPlayer player = players[i];
                        if (player != null)
                        {
                            string region = GetRegionFile(player);
                            if (IsInRegionReset(player))
                            {
                                player.Buffs.AddBuff(BuffResetZone);
                            }
                            else
                            {
                                if (player.Buffs.HasBuff(BuffResetZone))
                                    player.Buffs.RemoveBuff(BuffResetZone);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in Zones.Exec: {e.Message}");
            }
        }

        /// <summary>
        /// Renvoi une chaîne de caractères représentant la region où se trouve le joueur.
        /// </summary>
        /// <param name="player">Joueur</param>
        /// <returns><see cref="string"/></returns>
        public static string GetRegionFile(EntityPlayer player)
        {
            if (player != null)
            {
                double x = 0, z = 0;
                if (player.position.x < 0)
                {
                    x = Math.Truncate(player.position.x / 512) - 1;
                }
                else
                {
                    x = Math.Truncate(player.position.x / 512);
                }
                if (player.position.z < 0)
                {
                    z = Math.Truncate(player.position.z / 512) - 1;
                }
                else
                {
                    z = Math.Truncate(player.position.z / 512);
                }
                return x + "." + z;
            }
            return string.Empty;
        }

        /// <summary>
        /// Renvoi si oui ou non le joueur se trouve dans une region reset.
        /// </summary>
        /// <param name="player">Joueur</param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsInRegionReset(EntityPlayer player)
        {
            string region = GetRegionFile(player);
            List<string> regionsReset = PersistentContainer.Instance.RegionsReset;
            return regionsReset.Contains(region);
        }

        /// <summary>
        /// Renvoi si oui ou non le joueur se trouve chez un trader.
        /// </summary>
        /// <param name="player">Joueur</param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsInTraderArea(EntityPlayer player)
        {
            if (player != null)
            {
                PrefabInstance prefab = GameManager.Instance.World.GetPOIAtPosition(player.position);
                if (prefab != null)
                    return prefab.prefab.bTraderArea;
            }
            return false;
        }
    }
}