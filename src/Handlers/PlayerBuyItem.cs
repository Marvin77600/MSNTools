using AllocsFixes.NetConnections.Servers.Web;
using AllocsFixes.NetConnections.Servers.Web.API;
using MSNPersistentContainer = MSNTools.PersistentData.PersistentContainer;
using System.Net;
using System.Collections.Generic;

namespace MSNTools
{
    public class PlayerBuyItem : WebAPI
    {
        public override void HandleRequest(HttpListenerRequest _req, HttpListenerResponse _resp, WebConnection _user, int _permissionLevel)
        {
            if (_req.QueryString["shopID"] == null || _req.QueryString["steamID"] == null)
            {
                _resp.StatusCode = 400;
                Web.SetResponseTextContent(_resp, "No ShopID or SteamID given");
            }
            else
            {
                int shopID = int.Parse(_req.QueryString["shopID"]);
                string steamID = _req.QueryString["steamID"];
                _resp.SendChunked = true;

                List<Shop.Item> shopItems = Shop.ShopItems;
                Shop.Item shopItem = null;
                foreach (var item in shopItems)
                {
                    if (item.ID == shopID)
                        shopItem = item;
                }

                if (shopItem == null) return;

                var clientsList = PersistentOperations.ClientList();
                int playerId = -1;
                ClientInfo clientInfo = null;
                foreach (var client in clientsList)
                {
                    if (client.PlatformId.ToString() == steamID)
                    {
                        playerId = client.entityId;
                        clientInfo = client;
                        break;
                    }
                }
                if (clientInfo == null) return;
                
                shopItem.Purchase(clientInfo);
            }
        }

        public PlayerBuyItem() : base()
        {
        }
    }
}