using MSNTools.PersistentData;
using MSNTools.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSNTools.ChatCommands
{
    public class ChatCommandShop : ChatCommandAbstract
    {
        public override void Execute(List<string> _params, ClientInfo _clientInfo)
        {
            try
            {
                if (_clientInfo != null)
                {
                    MSNLocalization.Language language = PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].Language;

                    List<ShopStructure> shopList = PersistentContainer.Instance.ShopList;
                    List<string> list = new List<string>();
                    ChatCommandsHook.ChatMessage(_clientInfo, MSNLocalization.Get("shopResume", language), -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                    foreach (var item in shopList)
                    {
                        if (item.BuyerName == "Unknown")
                            ChatCommandsHook.ChatMessage(_clientInfo, $"• {item.GetID} - {Localization.Get(item.GetItemName)} - {item.GetItemDataSerializable.quality} - {item.GetPrice} - {item.GetSellerName}\n", -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "shop" };
    }
}
