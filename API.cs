using System;
using System.Collections.Generic;
using MSNTools.Discord;

namespace MSNTools
{
    public class API : IModApi
    {
        /// <summary>
        /// Défini la fréquence de rafraichissement de la méthode.
        /// </summary>

        public void InitMod()
        {
            ModEvents.GameUpdate.RegisterHandler(GameUpdate);
            ModEvents.SavePlayerData.RegisterHandler(SavePlayerData);
            ModEvents.ChatMessage.RegisterHandler(ChatMessage);
            ModEvents.PlayerSpawnedInWorld.RegisterHandler(PlayerSpawnedInWorld);
            ModEvents.PlayerDisconnected.RegisterHandler(PlayerDisconnected);
            ModEvents.GameStartDone.RegisterHandler(GameStartDone);
            ModEvents.GameShutdown.RegisterHandler(GameShutdown);
            Log.Out($"{Config.ModName} chargé");
        }

        private void GameUpdate()
        {
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
        }

        private static void PlayerSpawnedInWorld(ClientInfo _cInfo, RespawnType _respawnReason, Vector3i _pos)
        {
            if (_respawnReason.Equals(RespawnType.JoinMultiplayer))
                ModEventsDiscordBehaviour.PlayerSpawnedInWorld(_cInfo);
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
            ModEventsDiscordBehaviour.ChatMessage(_cInfo, _type, _msg);
            return true;
        }
    }
}