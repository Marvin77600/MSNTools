using AllocsFixes.JSON;
using AllocsFixes.NetConnections.Servers.Web;
using AllocsFixes.NetConnections.Servers.Web.API;
using AllocsFixes.PersistentData;
using MSNPersistentContainer = MSNTools.PersistentData.PersistentContainer;
using System.Net;
using System.Text;

namespace MSNTools
{
    public class GetPlayerData : WebAPI
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
                    if (player == null)
                    {
                        _resp.StatusCode = 404;
                        Web.SetResponseTextContent(_resp, "Unknown user id given");
                    }
                    else
                    {
                        JSONObject _root = WritePlayerData(player);
                        WriteJSON(_resp, _root);
                    }
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

        public GetPlayerData() : base()
        {
        }
    }
}