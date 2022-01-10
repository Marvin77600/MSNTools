using HarmonyX = HarmonyLib.Harmony;
using MSNTools.ChatCommands;
using MSNTools.Discord;
using MSNTools.PersistentData;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MSNTools
{
    public class API : IModApi
    {
        public void InitMod(Mod _modInstance)
        {
            ModEvents.SavePlayerData.RegisterHandler(SavePlayerData);
            ModEvents.ChatMessage.RegisterHandler(ChatMessage);
            ModEvents.PlayerSpawnedInWorld.RegisterHandler(PlayerSpawnedInWorld);
            ModEvents.PlayerDisconnected.RegisterHandler(PlayerDisconnected);
            ModEvents.GameStartDone.RegisterHandler(GameStartDone);
            ModEvents.GameShutdown.RegisterHandler(GameShutdown);
            ModEvents.GameAwake.RegisterHandler(GameAwake);
            CustomModEvents.AwardKill.RegisterHandler(AwardKill);
            Log.Out($"{Config.ModName} chargé");
            var harmony = new HarmonyX(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        private void GameAwake()
        {
            MSNLocalization.Init();
            ChatCommandsHook.RegisterChatCommands();
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
                        if (PersistentContainer.Instance.Players[cInfo.PlatformId.ToString()].IsDonator)
                        {
                            Log.Warning($"Don de {Bank.DonatorGainEveryHours} {Bank.DeviseName}");
                            PersistentContainer.Instance.Players[cInfo.PlatformId.ToString()].PlayerWallet += Bank.DonatorGainEveryHours;
                        }
                        else
                        {
                            PersistentContainer.Instance.Players[cInfo.PlatformId.ToString()].PlayerWallet += Bank.GainEveryHours;
                        }
                        PersistentContainer.DataChange = true;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out($"{Config.ModPrefix} Error in PatchProcessDamageEntityAlive.Postfix: {e.Message}");
            }
        }

        private static void SavePlayerData(ClientInfo _cInfo, PlayerDataFile _playerDataFile)
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
                Log.Out($"{Config.ModPrefix} Error in API.SavePlayerData: {e.Message}");
            }
        }

        private static void GameStartDone()
        {
            try
            {
                Config.Load();
                Timers.TimerStart();
                ModEventsDiscordBehaviour.GameStartDone();
                PersistentContainer.Instance.Load();
            }
            catch (Exception e)
            {
                Log.Out($"{Config.ModPrefix} Error in API.GameStartDone: {e.Message}");
            }
        }

        private static void GameShutdown()
        {
            Timers.TimerStop();
            ModEventsDiscordBehaviour.GameShutdown();
            PersistentContainer.Instance.Save();
        }

        private static void PlayerSpawnedInWorld(ClientInfo _cInfo, RespawnType _respawnReason, Vector3i _pos)
        {
            try
            {
                if (_cInfo != null && _cInfo.PlatformId != null)
                {
                    EntityPlayer player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
                    if (_respawnReason.Equals(RespawnType.JoinMultiplayer))
                    {
                        ModEventsDiscordBehaviour.PlayerSpawnedInWorld(_cInfo);
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
                Log.Out($"{Config.ModPrefix} Error in API.PlayerSpawnedInWorld: {e.Message}");
            }
        }

        private static void PlayerDisconnected(ClientInfo _cInfo, bool _bShutdown)
        {
            if (!_bShutdown)
                ModEventsDiscordBehaviour.PlayerDisconnected(_cInfo);
        }

        private static bool ChatMessage(ClientInfo _cInfo, EChatType _type, int _senderId, string _msg, string _mainName, bool _localizeMain, List<int> _recipientEntityIds)
        {
            if (_senderId == -1)
                return true;
            if (_msg.StartsWith(ChatCommandsHook.ChatCommandsPrefix) && ChatCommandsHook.ChatCommandsEnabled)
            {
                ChatCommandsHook.Exec(_cInfo, _msg);
                return false;
            }
            else
                ModEventsDiscordBehaviour.ChatMessage(_cInfo, _type, _msg);
            return true;
        }
    }
}