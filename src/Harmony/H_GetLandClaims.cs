using HarmonyLib;
using System;
using System.Net;
using System.Collections.Generic;
using AllocsFixes;
using AllocsFixes.PersistentData;
using AllocsFixes.JSON;
using AllocsFixes.NetConnections.Servers.Web.API;
using AllocsFixes.NetConnections.Servers.Web;

namespace MSNTools.Harmony
{
    public class H_GetLandClaims
    {
        [HarmonyPatch(typeof(GetLandClaims), "HandleRequest", new Type[] { typeof(HttpListenerRequest), typeof(HttpListenerResponse), typeof(WebConnection), typeof(int) })]
        public class H_GetLandClaims_HandleRequest
        {
            static bool Prefix(HttpListenerRequest _req, HttpListenerResponse _resp, WebConnection _user, int _permissionLevel)
            {
                PlatformUserIdentifierAbs _userIdentifier = null;
                if (_req.QueryString["userid"] != null && !PlatformUserIdentifierAbs.TryFromCombinedString(_req.QueryString["userid"], out _userIdentifier))
                {
                    _resp.StatusCode = 400;
                    Web.SetResponseTextContent(_resp, "Invalid user id given");
                }
                else
                {
                    PlatformUserIdentifierAbs userId = _user?.UserId;
                    bool flag = WebConnection.CanViewAllClaims(_permissionLevel);
                    JSONObject _root = new JSONObject();
                    _root.Add("claimsize", new JSONNumber(GamePrefs.GetInt(EnumUtils.Parse<EnumGamePrefs>("LandClaimSize"))));
                    JSONArray _node1 = new JSONArray();
                    _root.Add("claimowners", _node1);
                    LandClaimList.OwnerFilter[] _ownerFilters = null;
                    if (_userIdentifier != null || !flag)
                    {
                        if (_userIdentifier != null && !flag)
                            _ownerFilters = new LandClaimList.OwnerFilter[2]
                            {
                            LandClaimList.UserIdFilter(userId),
                            LandClaimList.UserIdFilter(_userIdentifier)
                            };
                        else if (!flag)
                            _ownerFilters = new LandClaimList.OwnerFilter[1]
                            {
                            LandClaimList.UserIdFilter(userId)
                            };
                        else
                            _ownerFilters = new LandClaimList.OwnerFilter[1]
                            {
                            LandClaimList.UserIdFilter(_userIdentifier)
                            };
                    }
                    LandClaimList.PositionFilter[] _positionFilters = null;
                    foreach (KeyValuePair<Player, List<Vector3i>> landClaim in LandClaimList.GetLandClaims(_ownerFilters, _positionFilters))
                    {
                        JSONObject _node2 = new JSONObject();
                        _node1.Add(_node2);
                        _node2.Add("steamid", new JSONString(landClaim.Key.PlatformId.CombinedString));
                        _node2.Add("claimactive", new JSONBoolean(landClaim.Key.LandProtectionActive));
                        if (landClaim.Key.Name.Length > 0)
                            _node2.Add("playername", new JSONString(landClaim.Key.Name));
                        else
                            _node2.Add("playername", new JSONNull());
                        JSONArray _node3 = new JSONArray();
                        _node2.Add("claims", _node3);
                        foreach (Vector3i vector3i in landClaim.Value)
                        {
                            JSONObject _node4 = new JSONObject();
                            _node4.Add("x", new JSONNumber(vector3i.x));
                            _node4.Add("y", new JSONNumber(vector3i.y));
                            _node4.Add("z", new JSONNumber(vector3i.z));
                            _node3.Add(_node4);
                        }
                    }
                    WebAPI.WriteJSON(_resp, _root);
                }
                return false;
            }
        }
    }
}