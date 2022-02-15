using LitJson;
using MSNTools.PersistentData;
using System;
using System.Net;
using System.Collections.Generic;

namespace MSNTools.ChatCommands
{
    public class ChatCommandVote : ChatCommandAbstract
    {
        public static bool IsEnabled = false;
        public static int GainPerVote = 0;
        public static string APIServerToken = "";

        ClientInfo clientInfo;
        DateTime utcNow;
        MSNLocalization.Language language;

        public override void Execute(List<string> _params, ClientInfo _clientInfo)
        {
            try
            {
                if (_clientInfo != null)
                {
                    language = PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].Language;
                    DateTime time = PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].LastVote;
                    utcNow = DateTime.UtcNow;
                    
                    if (DateTime.Compare(DateTime.UtcNow.AddHours(-2), time) > 0)
                    {
                        clientInfo = _clientInfo;
                        WebClient client = new WebClient();
                        string playerName = _clientInfo.playerName;
                        client.DownloadStringAsync(new Uri($"https://api.top-serveurs.net/v1/votes/claim-username?server_token={APIServerToken}&playername={playerName}"));
                        client.DownloadStringCompleted += Client_DownloadStringCompleted;
                    }
                    else
                    {
                        string response = MSNLocalization.Get("wait2Hours", language);
                        ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            JsonData jsonData = JsonMapper.ToObject(e.Result);
            if (HasVoted(jsonData[2].ToString()))
            {
                PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()].LastVote = utcNow;
                Bank.GiveMoney(clientInfo, GainPerVote);
                PersistentContainer.DataChange = true;

                string response = MSNLocalization.Get("voteClaimed", language).Replace("{0}", GainPerVote.ToString()).Replace("{1}", Bank.DeviseName);
                ChatCommandsHook.ChatMessage(clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
            }
            else
            {
                string response = MSNLocalization.Get("voteNotClaim", language);
                ChatCommandsHook.ChatMessage(clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
            }
        }

        bool HasVoted(string result)
        {
            if (result != null)
            {
                if (result == "1" || result == "2")
                {
                    return true;
                }
            }
            return false;
        }

        public override string[] GetCommands() => new string[] { "vote" };
    }
}
