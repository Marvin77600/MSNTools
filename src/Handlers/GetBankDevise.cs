using AllocsFixes.JSON;
using AllocsFixes.NetConnections.Servers.Web;
using AllocsFixes.NetConnections.Servers.Web.API;
using System;
using System.Net;

namespace MSNTools
{
    public class GetBankDevise : WebAPI
    {
        public override void HandleRequest(HttpListenerRequest _req, HttpListenerResponse _resp, WebConnection _user, int _permissionLevel)
        {
            try
            {
                JSONString _root = WriteBankDevise();
                WriteJSON(_resp, _root);
            }
            catch (Exception e)
            {
                MSNUtils.LogError("GetBankDevise " + e.Message);
            }
        }

        internal static JSONString WriteBankDevise()
        {
            JSONString money = new JSONString(Bank.DeviseName);
            return money;
        }

        public GetBankDevise() : base()
        {
        }
    }
}
