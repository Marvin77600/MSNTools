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
        public override void Execute(List<string> _params, ClientInfo _clientInfo)
        {
            try
            {
                if (_clientInfo != null)
                {
                    int day = GameUtils.WorldTimeToDays(GameManager.Instance.World.GetWorldTime());
                    int bloodMoonFrequency = GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency);
                    int modulo = day % GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency);

                    MSNLocalization.Language language = PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].Language;

                    if (modulo == 0)
                    {
                        string response = MSNLocalization.Get("bloodMoonTonight", language);
                        ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                    }
                    else
                    {
                        if (modulo == bloodMoonFrequency - 1)
                        {
                            string response = MSNLocalization.Get("bloodMoonTomorrow", language);
                            ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                        }
                        else
                        {
                            string response = MSNLocalization.Get("bloodMoonDay", language).Replace("{0}", (bloodMoonFrequency - modulo).ToString()).Replace("{1}", (day + bloodMoonFrequency - modulo).ToString());
                            ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "bloodmoon", "bd", "bloodday", "blood" };
    }
}