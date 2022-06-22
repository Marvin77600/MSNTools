using AllocsFixes.JSON;
using AllocsFixes.NetConnections.Servers.Web;
using AllocsFixes.NetConnections.Servers.Web.API;
using System;
using System.Net;
using System.Collections.Generic;

namespace MSNTools
{
    public class GetShopItems : WebAPI
    {
        public override void HandleRequest(HttpListenerRequest _req, HttpListenerResponse _resp, WebConnection _user, int _permissionLevel)
        {
            try
            {
                JSONObject _root = WriteShopList();
                WriteJSON(_resp, _root);
            }
            catch (Exception e)
            {
                MSNUtils.LogError("GetResetRegions " + e.Message);
            }
        }

        internal static JSONObject WriteShopList()
        {
            var shopItems = Shop.ShopItems;
            JSONObject jSONObject = new JSONObject();
            JSONArray jSONArray = new JSONArray();
            jSONObject.Add($"shopList", jSONArray);
            DoList(jSONArray, shopItems);
            return jSONObject;
        }

        private static void DoList(JSONArray array, List<Shop.Item> shopList)
        {
            for (int index = 0; index < shopList.Count; index++)
            {
                array.Add(GetJsonForItem(shopList[index]));
            }
        }

        private static JSONNode GetJsonForItem(Shop.Item _item)
        {
            if (_item == null)
                return new JSONNull();
            JSONObject jsonForItem = new JSONObject();
            jsonForItem.Add("id", new JSONNumber(_item.ID));
            jsonForItem.Add("count", new JSONNumber(_item.Count));
            jsonForItem.Add("itemName", new JSONString(_item.ItemName));
            jsonForItem.Add("displayName", new JSONString(Localization.Get(_item.ItemName)));
            jsonForItem.Add("quality", new JSONNumber(_item.Quality));
            jsonForItem.Add("description", new JSONString(_item.Description));
            jsonForItem.Add("price", new JSONNumber(_item.Price));
            return jsonForItem;
        }

        public GetShopItems() : base()
        {
        }
    }
}