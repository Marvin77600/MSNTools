using MSNTools.PersistentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSNTools.ChatCommands
{
    public class ChatCommandBloodMoonDay : ChatCommandAbstract
    {
        public override string Execute(List<string> _params, ClientInfo _clientInfo)
        {
            try
            {
                if (_clientInfo != null)
                {
                    int day = GameUtils.WorldTimeToDays(GameManager.Instance.World.GetWorldTime());
                    int bloodMoonFrequency = GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency);
                    int modulo = day % bloodMoonFrequency;

                    MSNLocalization.Language language = PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].Language;

                    if (modulo == 0)
                    {
                        string response = MSNLocalization.Get("bloodMoonTonight", language);
                        ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                        return response;
                    }
                    else
                    {
                        if (modulo == bloodMoonFrequency - 1)
                        {
                            string response = MSNLocalization.Get("bloodMoonTomorrow", language);
                            ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                            return response;
                        }
                        else
                        {
                            string response = MSNLocalization.Get("bloodMoonDay", language, bloodMoonFrequency - modulo, day + bloodMoonFrequency - modulo);
                            ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                            return response;
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

        public override string[] GetCommands() => new string[] { "bloodmoon", "bd", "bloodday", "blood" };
    }
}