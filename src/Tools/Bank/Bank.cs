using MSNTools.PersistentData;
using System;
using System.Collections.Generic;

namespace MSNTools
{
    public class Bank
    {
        public static bool IsEnabled = false;
        public static string DeviseName = "";
        public static int GainEveryHours = 2;
        public static int DonatorGainEveryHours = 3;

        public static void CheckGiveMoneyEveryTime()
        {
            try
            {
                if (GameManager.Instance.World != null && GameManager.Instance.World.Players.Count > 0 && GameManager.Instance.World.Players.dict.Count > 0)
                {
                    List<ClientInfo> clientsInfo = PersistentOperations.ClientList();
                    foreach (ClientInfo clientInfo in clientsInfo)
                    {
                        DateTime time = PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()].Time;

                        DateTime utcNow = DateTime.UtcNow;
                        if (DateTime.Compare(DateTime.UtcNow.AddHours(-1), time) > 0)
                        {
                            Log.Out($"{clientInfo.playerName} vient de gagner {GainEveryHours} {DeviseName}");
                            PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()].PlayerWallet += GainEveryHours;
                            PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()].Time = utcNow;
                            PersistentContainer.DataChange = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out($"{Config.ModPrefix} Error in Bank.CheckGiveMoneyEveryTime: {e.Message}");
            }
        }
    }
}