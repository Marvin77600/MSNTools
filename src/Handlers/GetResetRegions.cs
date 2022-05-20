using AllocsFixes.JSON;
using AllocsFixes.NetConnections.Servers.Web;
using AllocsFixes.NetConnections.Servers.Web.API;
using AllocsFixes.PersistentData;
using MSNPersistentContainer = MSNTools.PersistentData.PersistentContainer;
using System;
using System.Net;

namespace MSNTools
{
    public class GetResetRegions : WebAPI
    {
        public override void HandleRequest(HttpListenerRequest _req, HttpListenerResponse _resp, WebConnection _user, int _permissionLevel)
        {
            try
            {
                JSONObject _root = WritePlayerMoney();
                WriteJSON(_resp, _root);
            }
            catch (Exception e)
            {
                MSNUtils.LogError("GetResetRegions " + e.Message);
            }
        }

        internal static JSONObject WritePlayerMoney()
        {
            var list = MSNPersistentContainer.Instance.RegionsReset;
            JSONObject jSONObject = new JSONObject();
            for (int i = 0; i < list.Count; i++)
            {
                var region = list[i];
                jSONObject.Add($"{i}", new JSONString(region));
            }
            return jSONObject;
        }

        public GetResetRegions() : base()
        {
        }
    }
}
