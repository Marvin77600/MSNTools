using HarmonyX = HarmonyLib.Harmony;
using MSNTools.ChatCommands;
using MSNTools.Discord;
using MSNTools.PersistentData;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MSNTools
{
    public class API : IModApi
    {
        public static Mod Mod;

        public void InitMod(Mod _modInstance)
        {
            try
            {
                Mod = _modInstance;
                ModEvents.SavePlayerData.RegisterHandler(SavePlayerData);
                ModEvents.ChatMessage.RegisterHandler(ChatMessage);
                ModEvents.PlayerSpawnedInWorld.RegisterHandler(PlayerSpawnedInWorld);
                //ModEvents.PlayerLogin.RegisterHandler(PlayerLogin);
                ModEvents.PlayerDisconnected.RegisterHandler(PlayerDisconnected);
                ModEvents.GameStartDone.RegisterHandler(GameStartDone);
                ModEvents.GameShutdown.RegisterHandler(GameShutdown);
                ModEvents.GameAwake.RegisterHandler(GameAwake);
                CustomModEvents.AwardKill.RegisterHandler(AwardKill);
                CustomModEvents.BlockChange.RegisterHandler(BlockChange);
                CustomModEvents.StartBloodMoon.RegisterHandler(StartBloodMoon);
                CustomModEvents.EndBloodMoon.RegisterHandler(EndBloodMoon);
                MSNUtils.Log($"Mod chargé");
                var harmony = new HarmonyX(GetType().ToString());
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.InitMod: {e.Message}");
            }
        }

        private bool PlayerLogin(ClientInfo _cInfo, string _message, StringBuilder _stringBuild)
        {
            try
            {
                if (PersistentOperations.IsBloodmoon() && _cInfo != null)
                {
                    MSNLocalization.Language language = PersistentContainer.Instance.Players[_cInfo.PlatformId.ToString()].Language;
                    string response = MSNLocalization.Get("noEnoughMoney", language);
                    PlayerDataFile pdf = PersistentOperations.GetPlayerDataFileFromUId(_cInfo.PlatformId);
                    if (pdf != null)
                    {
                        if (pdf.totalTimePlayed < 5)
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"kick {_cInfo.CrossplatformId.CombinedString} \"{response}\"", null);
                        }
                    }
                    else
                    {
                        SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"kick {_cInfo.CrossplatformId.CombinedString} \"{response}\"", null);
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.PlayerLogin: {e.Message}");
            }
            return true;
        }

        private void GameAwake()
        {
            try
            {
                PersistentContainer.Instance.Load();
                Config.Load();
                MSNLocalization.Init();
                ChatCommandsHook.RegisterChatCommands();
                ResetRegions.Exec();
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.GameAwake: {e.Message}");
                throw;
            }
        }

        private void AwardKill(EntityAlive __instance, EntityAlive killer)
        {
            try
            {
                if (__instance is EntityZombie && killer != __instance && killer != null && killer is EntityPlayer)
                {
                    EntityPlayer player = (EntityPlayer)killer;
                    ClientInfo cInfo = PersistentOperations.GetClientInfoFromEntityId(player.entityId);
                    if (cInfo != null)
                    {
                        if (!Bank.HaveMaxMoney(cInfo))
                        {
                            if (PersistentContainer.Instance.Players[cInfo.PlatformId.ToString()].IsDonator)
                            {
                                MSNUtils.LogWarning($"Don de {Bank.DonatorGainEveryHours} {Bank.DeviseName}");
                                Bank.GiveMoney(cInfo, Bank.DonatorGainEveryHours);
                            }
                            else
                            {
                                Bank.GiveMoney(cInfo, Bank.GainEveryHours);
                            }
                            PersistentContainer.DataChange = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.AwardKill: {e.Message}");
            }
        }
        
        private void SavePlayerData(ClientInfo _cInfo, PlayerDataFile _playerDataFile)
        {
            try
            {
                if (_cInfo != null && _playerDataFile != null)
                {
                    //if (HighPingKicker.IsEnabled)
                    //{
                    //    HighPingKicker.Exec(_cInfo);
                    //}
                    if (InventoryChecks.IsEnabled)
                    {
                        InventoryChecks.CheckInv(_cInfo, _playerDataFile);
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.SavePlayerData: {e.Message}");
            }
        }

        private void BlockChange(GameManager _gameManager, PlatformUserIdentifierAbs _platformUserIdentifierAbs, List<BlockChangeInfo> _blockChangeInfos)
        {
            CheckChangeBlocks.Exec(_gameManager, _platformUserIdentifierAbs, _blockChangeInfos);
        }

        private void StartBloodMoon()
        {
            try
            {
                if (BloodMoonAlerts.IsEnabled)
                    DiscordWebhookSender.SendBloodMoonStart();
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.StartBloodMoon: {e.Message}");
            }
        }

        private void EndBloodMoon()
        {
            try
            {
                if (BloodMoonAlerts.IsEnabled)
                {
                    DiscordWebhookSender.SendBloodMoonEnd();
                    BloodMoonAlerts.AlertAlreadySent = false;
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.EndBloodMoon: {e.Message}");
            }
        }

        private void GameStartDone()
        {
            try
            {
                Timers.TimerStart();
                ModEventsDiscordBehaviour.GameStartDone();
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.GameStartDone: {e.Message}");
            }
        }

        private void GameShutdown()
        {
            try
            {
                Timers.TimerStop();
                ModEventsDiscordBehaviour.GameShutdown();
                PersistentContainer.Instance.Save();
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.GameShutdown: {e.Message}");
            }
        }

        private void PlayerSpawnedInWorld(ClientInfo _cInfo, RespawnType _respawnReason, Vector3i _pos)
        {
            try
            {
                if (_cInfo != null && _cInfo.PlatformId != null)
                {
                    if (_respawnReason.Equals(RespawnType.JoinMultiplayer))
                    {
                        ModEventsDiscordBehaviour.PlayerSpawnedInWorld(_cInfo);
                        ChatCommandBuy.NotifSellerWhenConnected(_cInfo);
                        PersistentContainer.DataChange = true;
                    }
                    else if (_respawnReason.Equals(RespawnType.EnterMultiplayer))
                    {
                        PersistentContainer.Instance.Players[_cInfo.PlatformId.ToString()].PlayerName = _cInfo.playerName;
                        PersistentContainer.Instance.Players[_cInfo.PlatformId.ToString()].Language = MSNLocalization.Language.French;
                        PersistentContainer.Instance.Players[_cInfo.PlatformId.ToString()].PlayerWallet = 0;
                        PersistentContainer.Instance.Players[_cInfo.PlatformId.ToString()].Time = DateTime.UtcNow;
                        PersistentContainer.Instance.Players[_cInfo.PlatformId.ToString()].LastVote = new DateTime();
                        PersistentContainer.Instance.Players[_cInfo.PlatformId.ToString()].TPPositions = new Dictionary<string, string>();
                        PersistentContainer.DataChange = true;
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.PlayerSpawnedInWorld: {e.Message}");
            }
        }

        private void PlayerDisconnected(ClientInfo _cInfo, bool _bShutdown)
        {
            try
            {
                if (!_bShutdown)
                    ModEventsDiscordBehaviour.PlayerDisconnected(_cInfo);
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.PlayerDisconnected: {e.Message}");
                throw;
            }
        }

        private bool ChatMessage(ClientInfo _cInfo, EChatType _type, int _senderId, string _msg, string _mainName, bool _localizeMain, List<int> _recipientEntityIds)
        {
            try
            {
                if (_senderId == -1)
                    return true;

                if (ChatCommandTPToServerLocations.Exec(_cInfo, _msg))
                    return false;
                
                if (_msg.StartsWith(ChatCommandsHook.ChatCommandsPrefix) && ChatCommandsHook.ChatCommandsEnabled)
                {
                    ChatCommandsHook.Exec(_cInfo, _msg);
                    return false;
                }
                else
                    ModEventsDiscordBehaviour.ChatMessage(_cInfo, _type, _msg);
                return true;
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.ChatMessage: {e.Message}");
            }
            return false;
        }
    }
}