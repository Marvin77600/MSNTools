using MSNTools.PersistentData;
using MSNTools.ChatCommands;
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
        public static int Hours = 1;
        public static int Max = 1;

        /// <summary>
        /// Give de l'argent aux joueurs toutes les X heures.
        /// </summary>
        public static void CheckGiveMoneyEveryTime()
        {
            try
            {
                if (GameManager.Instance.World != null && GameManager.Instance.World.Players.Count > 0 && GameManager.Instance.World.Players.dict.Count > 0)
                {
                    List<ClientInfo> clientsInfo = PersistentOperations.ClientList();
                    foreach (ClientInfo clientInfo in clientsInfo)
                    {
                        if (!HaveMaxMoney(clientInfo))
                        {
                            DateTime time = PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()].Time;

                            DateTime utcNow = DateTime.UtcNow;
                            if (DateTime.Compare(DateTime.UtcNow.AddHours(-Hours), time) > 0)
                            {
                                MSNUtils.LogWarning($"{clientInfo.playerName} vient de gagner {GainEveryHours} {DeviseName}");
                                GiveMoney(clientInfo, DonatorGainEveryHours);
                                PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()].Time = utcNow;
                                PersistentContainer.DataChange = true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in Bank.CheckGiveMoneyEveryTime: {e.Message}");
            }
        }

        /// <summary>
        /// Give de l'argent à un joueur.
        /// </summary>
        /// <param name="clientInfo">Infos client du joueur</param>
        /// <param name="value">Montant</param>
        public static void GiveMoney(ClientInfo clientInfo, int value)
        {
            try
            {
                if (clientInfo != null)
                {
                    PersistentPlayer persistentPlayer = PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()];
                    int playerWallet = persistentPlayer.PlayerWallet;
                    if (Max - playerWallet < value)
                    {
                        PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()].PlayerWallet += Max - playerWallet;
                    }
                    else
                    {
                        PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()].PlayerWallet += value;
                    }
                    PersistentContainer.DataChange = true;
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in Bank.GiveMoney: {e.Message}");
            }
        }

        /// <summary>
        /// Renvoi si oui ou non le joueur peut avoir de l'argent.
        /// </summary>
        /// <param name="clientInfo">Infos client du joueur</param>
        /// <param name="value">Montant</param>
        /// <returns><see cref="int"/></returns>
        public static int CanTakeMoney(ClientInfo clientInfo, int value)
        {
            try
            {
                if (clientInfo != null)
                {
                    PersistentPlayer persistentPlayer = PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()];
                    int playerWallet = persistentPlayer.PlayerWallet;
                    if (Max - playerWallet < value)
                    {
                        MSNUtils.LogWarning($"CanTakeMoney() Max - playerWallet");
                        return Max - playerWallet;
                    }
                    else
                    {
                        MSNUtils.LogWarning($"CanTakeMoney() value");
                        return value;
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in Bank.CanTakeMoney: {e.Message}");
            }
            return 0;
        }

        /// <summary>
        /// Renvoi si oui ou non le joueur a assez d'argent.
        /// </summary>
        /// <param name="clientInfo">Infos client du joueur</param>
        /// <param name="value">Montant</param>
        /// <returns><see cref="bool"/></returns>
        public static bool HasEnoughMoney(ClientInfo clientInfo, int value)
        {
            MSNLocalization.Language language = PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()].Language;
            if (PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()].PlayerWallet >= value)
                return true;
            else
            {
                string response = MSNLocalization.Get("noEnoughMoney", language);
                ChatCommandsHook.ChatMessage(clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                return false;
            }
        }

        /// <summary>
        /// Renvoi si oui ou non le joueur a atteint le maximum d'argent.
        /// </summary>
        /// <param name="clientInfo">Infos client du joueur</param>
        /// <returns><see cref="bool"/></returns>
        public static bool HaveMaxMoney(ClientInfo clientInfo)
        {
            if (PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()].PlayerWallet >= Max)
                return true;
            else
                return false;
        }
    }
}