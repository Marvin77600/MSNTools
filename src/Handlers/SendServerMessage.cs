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
using MSNTools.ChatCommands;

namespace MSNTools
{
    public class SendServerMessage : WebAPI
    {
        public override void HandleRequest(HttpListenerRequest _req, HttpListenerResponse _resp, WebConnection _user, int _permissionLevel)
        {
            if (_req.QueryString["message"] == null || _req.QueryString["sender"] == null)
            {
                _resp.StatusCode = 400;
                Web.SetResponseTextContent(_resp, "No user id given");
            }
            else
            {
                string message = _req.QueryString["message"];
                string senderName = _req.QueryString["sender"];
                GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, message, senderName, false, null);
            }
        }

        public SendServerMessage() : base()
        {
        }
    }
}