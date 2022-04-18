using MSNTools.PersistentData;
using System;
using System.Collections.Generic;

namespace MSNTools.ChatCommands
{
    public class ChatCommandTransfert : ChatCommandAbstract
    {
        public override string Execute(List<string> _params, ClientInfo _clientInfo)
        {
            try
            {
                if (_clientInfo != null)
                {
                    if (_params.Count == 2)
                    {
                        MSNLocalization.Language clientLanguage = PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].Language;
                        string targetName = _params[0];
                        int targetEntityID = 0;
                        int amount = int.Parse(_params[1]);
                        var players = GameManager.Instance.World.Players.list;

                        foreach (var player in players)
                        {
                            if (player.EntityName.EqualsCaseInsensitive(targetName))
                            {
                                targetEntityID = player.entityId;
                                continue;
                            }
                        }

                        ClientInfo targetClientInfo = PersistentOperations.GetClientInfoFromEntityId(targetEntityID);
                        if (targetClientInfo != null)
                        {
                            MSNLocalization.Language targetLanguage = PersistentContainer.Instance.Players[targetClientInfo.PlatformId.ToString()].Language;
                            int walletAmount = PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].PlayerWallet;
                            int targetWalletAmount = PersistentContainer.Instance.Players[targetClientInfo.PlatformId.ToString()].PlayerWallet;

                            if (Bank.HasEnoughMoney(_clientInfo, amount))
                            {
                                int newAmount = Bank.CanTakeMoney(targetClientInfo, amount);

                                ChatCommandsHook.ChatMessage(_clientInfo, MSNLocalization.Get("transfertCompleteClientMessage", clientLanguage, newAmount, Bank.DeviseName, targetClientInfo.playerName), -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);

                                ChatCommandsHook.ChatMessage(targetClientInfo, MSNLocalization.Get("transfertCompleteTargetMessage", targetLanguage, _clientInfo.playerName, newAmount, Bank.DeviseName), -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);

                                PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].PlayerWallet -= newAmount;
                                Bank.GiveMoney(targetClientInfo, newAmount);
                                PersistentContainer.DataChange = true;
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
            return null;
        }

        public override string[] GetCommands() => new string[] { "transfert" };
    }
}