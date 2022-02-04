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

        public static void Exec()
        {
            try
            {
                if (GameManager.Instance.World != null && GameManager.Instance.World.Players.Count > 0 && GameManager.Instance.World.Players.dict.Count > 0)
                {
                    List<string> regionsReset = PersistentContainer.Instance.RegionsReset;
                    List<EntityPlayer> players = GameManager.Instance.World.Players.list;

                    for (int i = 0; i < players.Count; i++)
                    {
                        EntityPlayer player = players[i];
                        if (player != null)
                        {
                            double regionX = 0, regionZ = 0;
                            string region = GetRegionFile(player, regionX, regionZ);
                            if (regionsReset.Contains(region))
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

        public static string GetRegionFile(EntityPlayer player, double x, double z)
        {
            if (player != null)
            {
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
    }
}