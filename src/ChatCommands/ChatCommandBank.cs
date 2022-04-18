using MSNTools.PersistentData;
using System;
using System.Collections.Generic;

namespace MSNTools.ChatCommands
{
    public class ChatCommandBank : ChatCommandAbstract
    {
        public override string Execute(List<string> _params, ClientInfo _clientInfo)
        {
            try
            {
                if (_clientInfo != null)
                {
                    MSNLocalization.Language language = PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].Language;
                    if (!Bank.IsEnabled)
                    {
                        return null;
                    }
                    if (_params.Count == 0)
                    {
                        int moneyValue = PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].PlayerWallet;
                        string response = MSNLocalization.Get("moneyAmount", language, moneyValue, Bank.DeviseName);
                        ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                        return response;
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
            return null;
        }

        public override string[] GetCommands() => new string[] { "bank" };
    }
}