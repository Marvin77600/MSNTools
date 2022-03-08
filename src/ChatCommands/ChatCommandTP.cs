﻿using MSNTools.PersistentData;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MSNTools.ChatCommands
{
    public class ChatCommandTP : ChatCommandAbstract
    {
        public static bool IsEnabled = false;
        public static int TPCost = 1;
        public static int TPMaxCount = 1;

        public override void Execute(List<string> _params, ClientInfo _clientInfo)
        {
            try
            {
                if (IsEnabled)
                {
                    if (_clientInfo != null)
                    {
                        EntityPlayer entityPlayer = GameManager.Instance.World.Players.dict[_clientInfo.entityId];
                        Dictionary<string, string> tpPositions = PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].TPPositions;
                        string playerPosition = $"{(int)entityPlayer.position.x}, {(int)entityPlayer.position.y}, {(int)entityPlayer.position.z}";
                        MSNLocalization.Language language = PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].Language;

                        if (_params.Count == 1)
                        {
                            if (_params[0].ContainsCaseInsensitive("bed") || _params[0].ContainsCaseInsensitive("lit"))
                            {
                                if (entityPlayer.SpawnPoints.Count == 0)
                                {
                                    string response = MSNLocalization.Get("noBed", language);
                                    ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                                }
                                else
                                {
                                    Vector3i bedrollPos = entityPlayer.SpawnPoints.GetPos();
                                    PrefabInstance prefab = GameManager.Instance.World.GetPOIAtPosition(bedrollPos.ToVector3());
                                    if (prefab == null || prefab.prefab.PrefabName.StartsWith("rwg"))
                                    {
                                        string pos = $"{bedrollPos.x}, {bedrollPos.y}, {bedrollPos.z}";
                                        string[] vs = pos.Split(new string[] { ",", ", ", " ," }, StringSplitOptions.RemoveEmptyEntries);
                                        if (entityPlayer.AttachedToEntity)
                                        {
                                            string response = MSNLocalization.Get("onVehicle", language);
                                            ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                                        }
                                        else
                                        {
                                            if (Bank.HasEnoughMoney(_clientInfo, TPCost))
                                            {
                                                PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].PlayerWallet -= TPCost;
                                                PersistentContainer.DataChange = true;
                                                SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"teleportplayer {entityPlayer.entityId} {string.Join(" ", vs.ToArray())}", _clientInfo);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string response = MSNLocalization.Get("bedInPOI", language);
                                        ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                                    }
                                }
                            }
                            else if (_params[0].ContainsCaseInsensitive("list"))
                            {
                                if (tpPositions.Count == 0)
                                {
                                    string response = MSNLocalization.Get("noTPPoint", language);
                                    ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                                }
                                else
                                {
                                    foreach (KeyValuePair<string, string> tpPosition in tpPositions)
                                    {
                                        ChatCommandsHook.ChatMessage(_clientInfo, $"{tpPosition.Key} : {tpPosition.Value}", -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                                    }
                                }
                            }
                            else if (tpPositions.TryGetValue(_params[0], out string position))
                            {
                                string[] vs = position.Split(new string[] { ",", ", ", " ," }, StringSplitOptions.RemoveEmptyEntries);
                                if (tpPositions.ContainsKey(_params[0]))
                                {
                                    if (entityPlayer.AttachedToEntity) 
                                    {
                                        string response = MSNLocalization.Get("onVehicle", language);
                                        ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                                    }
                                    else
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"teleportplayer {entityPlayer.entityId} {string.Join(" ", vs.ToArray())}", _clientInfo);
                                    }
                                }
                            }
                        }
                        if (_params.Count == 2)
                        {
                            if (_params[0].ContainsCaseInsensitive("add"))
                            {
                                if (tpPositions.Count < TPMaxCount)
                                {
                                    PrefabInstance prefab = GameManager.Instance.World.GetPOIAtPosition(entityPlayer.position);
                                    if (prefab == null || prefab.prefab.PrefabName.StartsWith("rwg"))
                                    {
                                        if (tpPositions.ContainsKey(_params[1]))
                                        {
                                            string response = MSNLocalization.Get("sameTPPoint", language, _params[1]);
                                            ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                                            return;
                                        }
                                        else
                                        {
                                            tpPositions.Add(_params[1], playerPosition);
                                            string response = MSNLocalization.Get("addTPPoint", language, _params[1], playerPosition);
                                            ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                                            PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].TPPositions = tpPositions;
                                            PersistentContainer.DataChange = true;
                                            return;
                                        }
                                    }
                                    else if (Zones.IsInRegionReset(entityPlayer))
                                    {
                                        string response = MSNLocalization.Get("noTPInResetRegion", language);
                                        ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                                    }
                                    else
                                    {
                                        string response = MSNLocalization.Get("noTPInPOI", language);
                                        ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                                    }
                                }
                                else
                                {
                                    string response = MSNLocalization.Get("maxTPPoint", language);
                                    ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                                }
                            }
                            if (_params[0].ContainsCaseInsensitive("del") || _params[0].ContainsCaseInsensitive("remove"))
                            {
                                if (tpPositions.Count > 0)
                                {
                                    if (tpPositions.ContainsKey(_params[1]))
                                    {
                                        tpPositions.Remove(_params[1]);
                                        string response = MSNLocalization.Get("removeTPPoint", language, _params[1]);
                                        ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                                        PersistentContainer.Instance.Players[_clientInfo.PlatformId.ToString()].TPPositions = tpPositions;
                                        PersistentContainer.DataChange = true;
                                    }
                                    else
                                    {
                                        string response = MSNLocalization.Get("notTPPointFound", language, _params[1]);
                                        ChatCommandsHook.ChatMessage(_clientInfo, response, -1, $"{Config.Chat_Response_Color}{Config.Server_Response_Name}[-]", EChatType.Whisper, null);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "tp" };
    }
}