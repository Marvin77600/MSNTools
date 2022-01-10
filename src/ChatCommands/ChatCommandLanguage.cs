using MSNTools.PersistentData;
using System;
using System.Collections.Generic;

namespace MSNTools.ChatCommands
{
    public class ChatCommandLanguage : ChatCommandAbstract
    {
        public override void Execute(List<string> _params, ClientInfo _clientInfo)
        {
            try
            {
                if (_clientInfo != null)
                {
                    MSNLocalization.Language language = PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].Language;
                    if (!(_params[0] == "french" || _params[0] == "english"))
                    {
                        string response = MSNLocalization.Get("badlanguage", language).Replace("{0}", _params[0]);
                        ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                    }
                    else
                    {
                        Log.Out($"{language}");
                        language = StringToLanguageEnum(_params[0]);
                        PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].Language = StringToLanguageEnum(_params[0]);
                        PersistentContainer.DataChange = true;
                        string response = MSNLocalization.Get("newLanguage", language).Replace("{0}", _params[0]);
                        ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out($"Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "language" };

        private MSNLocalization.Language StringToLanguageEnum(string language)
        {
            if (language == "french")
                return MSNLocalization.Language.French;
            if (language == "english")
                return MSNLocalization.Language.English;
            return MSNLocalization.Language.French;
        }
    }
}