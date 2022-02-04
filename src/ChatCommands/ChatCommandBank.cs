using MSNTools.PersistentData;
using System;
using System.Collections.Generic;

namespace MSNTools.ChatCommands
{
    public class ChatCommandBank : ChatCommandAbstract
    {
        public override void Execute(List<string> _params, ClientInfo _clientInfo)
        {
            try
            {
                if (_clientInfo != null)
                {
                    MSNLocalization.Language language = PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].Language;
                    if (!Bank.IsEnabled)
                    {
                        return;
                    }
                    if (_params.Count == 0)
                    {
                        int moneyValue = PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].PlayerWallet;
                        string response = MSNLocalization.Get("moneyAmount", language).Replace("{0}", moneyValue.ToString()).Replace("{1}", Bank.DeviseName);
                        ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                    }
                    if (GameManager.Instance.adminTools.GetUserPermissionLevel(_clientInfo) < 1)
                    {
                        if (_params.Count == 2)
                        {
                            int value = int.Parse(_params[1]);
                            if (_params[0] == "add")
                            {
                                PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].PlayerWallet += value;
                            }
                            if (_params[0] == "remove")
                            {
                                int moneyValue = PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].PlayerWallet;
                                if (int.Parse(_params[1]) > moneyValue)
                                {
                                    string response = MSNLocalization.Get("notEnoughMoney", language);
                                    ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                                }
                                else
                                    PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].PlayerWallet -= value;
                            }

                            PersistentContainer.DataChange = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "bank" };
    }
}