using AllocsFixes.JSON;
using AllocsFixes.NetConnections.Servers.Web;
using AllocsFixes.NetConnections.Servers.Web.API;
using AllocsFixes.PersistentData;
using MSNPersistentContainer = MSNTools.PersistentData.PersistentContainer;
using MSNTools.Discord;
using MSNTools.PersistentData;
using System.Net;
using System.Text;
using PersistentContainer = AllocsFixes.PersistentData.PersistentContainer;

namespace MSNTools
{
    public class SendPlayerDataDiscord : WebAPI
    {
        public override void HandleRequest(HttpListenerRequest _req, HttpListenerResponse _resp, WebConnection _user, int _permissionLevel)
        {
            if (_req.QueryString["userid"] == null)
            {
                _resp.StatusCode = 400;
                Web.SetResponseTextContent(_resp, "No user id given");
            }
            else
            {
                string str = _req.QueryString["userid"];
                PlatformUserIdentifierAbs _userIdentifier;
                if (!PlatformUserIdentifierAbs.TryFromCombinedString(str, out _userIdentifier))
                {
                    _resp.StatusCode = 400;
                    Web.SetResponseTextContent(_resp, "Invalid user id given");
                }
                else
                {
                    Player player = PersistentContainer.Instance.Players[_userIdentifier, false];
                    PersistentPlayer playerData = MSNPersistentContainer.Instance.Players[player.PlatformId.ToString()];
                    DiscordWebhookSender.SendPlayerData(playerData);
                }
            }
        }

        internal static JSONObject WritePlayerData(Player player)
        {
            var playerData = MSNPersistentContainer.Instance.Players[player.PlatformId.ToString()];
            JSONObject jSONObject = new JSONObject();
            JSONArray jSONArray = new JSONArray();
            jSONObject.Add("playerData", jSONArray);
            jSONArray.Add(new JSONString(playerData.Language.ToString()));
            jSONArray.Add(new JSONBoolean(playerData.IsDonator));
            jSONArray.Add(new JSONNumber(playerData.PlayerWallet));
            jSONArray.Add(new JSONString(playerData.PlayerName));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var tpPosition in playerData.TPPositions)
            {
                stringBuilder.Append($"• {tpPosition.Key} - {tpPosition.Value}\n");
            }
            stringBuilder.ToString().TrimEnd('\n');
            jSONArray.Add(new JSONString(stringBuilder.ToString()));
            return jSONObject;
        }

        public SendPlayerDataDiscord() : base()
        {
        }
    }
}
