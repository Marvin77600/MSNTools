using AllocsFixes.JSON;
using AllocsFixes.NetConnections.Servers.Web;
using AllocsFixes.NetConnections.Servers.Web.API;
using AllocsFixes.PersistentData;
using MSNPersistentContainer = MSNTools.PersistentData.PersistentContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MSNTools
{
    public class GetPlayerMoney : WebAPI
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
                        JSONNumber _root = WritePlayerMoney(player);
                        WriteJSON(_resp, _root);
                    }
                }
            }
        }

        internal static JSONNumber WritePlayerMoney(Player player)
        {
            PlatformUserIdentifierAbs platformUserIdentifierAbs = player.PlatformId;
            JSONNumber money = new JSONNumber(MSNPersistentContainer.Instance.Players[platformUserIdentifierAbs.ToString()].PlayerWallet);
            return money;
        }

        public GetPlayerMoney() : base()
        {
        }
    }
}
