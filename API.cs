using HarmonyX = HarmonyLib.Harmony;
using MSNTools.ChatCommands;
using MSNTools.Discord;
using MSNTools.PersistentData;
using MSNTools.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MSNTools
{
    public class API : IModApi
    {
        public static Mod Mod;

        /// <summary>
        /// Initialisation du mod.
        /// </summary>
        /// <param name="_modInstance">?</param>
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
                CustomModEvents.PlayerKillPlayer.RegisterHandler(PlayerKillPlayer);
                MSNUtils.Log($"Mod chargé");
                HarmonyX.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.InitMod: {e.Message}");
            }
        }

        /// <summary>
        /// Méthode s'exécutant lorsqu'un joueur se log au serveur.
        /// </summary>
        /// <param name="_cInfo">Infos client du joueur</param>
        /// <param name="_message">Message qui apparaît sur l'écran</param>
        /// <param name="_stringBuild">?</param>
        /// <returns><see cref="bool"/></returns>
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

        /// <summary>
        /// Méthode s'exécutant lorsque le jeu démarre.
        /// </summary>
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

        /// <summary>
        /// Méthode s'exécutant lorsqu'une entité meurt.
        /// </summary>
        /// <param name="__instance">L'entité qui s'est faite tuée</param>
        /// <param name="killer">L'entité qui a tuée</param>
        private void AwardKill(EntityAlive __instance, EntityAlive killer)
        {
            try
            {
                EntityAlive target = __instance;

                // Player kill zombie
                if (target is EntityZombie && killer != target && killer != null && killer is EntityPlayer)
                {
                    EntityPlayer player = (EntityPlayer)killer;
                    ClientInfo cInfo = PersistentOperations.GetClientInfoFromEntityId(player.entityId);
                    if (cInfo != null)
                    {
                        if (!Bank.HaveMaxMoney(cInfo))
                        {
                            if (PersistentContainer.Instance.Players[cInfo.PlatformId.ToString()].IsDonator)
                            {
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

        /// <summary>
        /// Méthode s'exécutant lors de la sauvegarde du profil joueur.
        /// </summary>
        /// <param name="_cInfo">Infos client du joueur</param>
        /// <param name="_playerDataFile">Sauvegarde du profil joueur</param>
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
                    if (InventoryChecks.IsEnabled) InventoryChecks.CheckInv(_cInfo, _playerDataFile);
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.SavePlayerData: {e.Message}");
            }
        }

        /// <summary>
        /// Méthode s'exécutant lors de la pose d'un bloc.
        /// </summary>
        /// <param name="_gameManager">?</param>
        /// <param name="_platformUserIdentifierAbs">Données de la plateforme du joueur</param>
        /// <param name="_blockChangeInfos">Liste des blocs posés</param>
        private void BlockChange(GameManager _gameManager, PlatformUserIdentifierAbs _platformUserIdentifierAbs, List<BlockChangeInfo> _blockChangeInfos)
        {
            try
            {
                CheckChangeBlocks.Exec(_gameManager, _platformUserIdentifierAbs, _blockChangeInfos);
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.BlockChange: {e.Message}");
            }
        }

        /// <summary>
        /// Méthode s'exécutant au démarrage d'une bloodmoon.
        /// </summary>
        private void StartBloodMoon()
        {
            try
            {
                if (BloodMoonAlerts.IsEnabled) DiscordWebhookSender.SendBloodMoonStart();
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.StartBloodMoon: {e.Message}");
            }
        }

        /// <summary>
        /// Méthode s'exécutant à la fin d'une bloodmoon.
        /// </summary>
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

        /// <summary>
        /// Méthode s'exécutant lorsqu'un joueur tue un autre joueur.
        /// </summary>
        /// <param name="_mainEntity">Joueur tué</param>
        /// <param name="_otherEntity">Joueur tueur</param>
        private void PlayerKillPlayer(EntityAlive _mainEntity, EntityAlive _otherEntity)
        {
            try
            {
                if (_mainEntity != null && _otherEntity != null)
                {
                    if (Zones.IsInTraderArea(_mainEntity as EntityPlayer) && PlayerInvulnerabilityAtTrader.IsEnabled)
                    {
                        ClientInfo killerClientInfo, targetClientInfo;
                        killerClientInfo = PersistentOperations.GetClientInfoFromEntityId(_otherEntity.entityId);
                        targetClientInfo = PersistentOperations.GetClientInfoFromEntityId(_mainEntity.entityId);
                        DiscordWebhookSender.SendAlertPlayerKilledAtTrader(killerClientInfo, targetClientInfo);
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.PlayerKillPlayer: {e.Message}");
            }
        }

        /// <summary>
        /// Méthode s'exécutant lorsque le serveur est en ligne.
        /// </summary>
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

        /// <summary>
        /// Méthode s'exécutant lorsque le serveur s'éteint.
        /// </summary>
        private void GameShutdown()
        {
            try
            {
                Timers.TimerStop();
                ModEventsDiscordBehaviour.GameShutdown();
                if (ClearVehicles.IsEnabled) ClearVehicles.Start();
                PersistentContainer.Instance.Save();
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.GameShutdown: {e.Message}");
            }
        }

        /// <summary>
        /// Méthode s'exécutant lorsqu'un joueur apparaît sur le serveur.
        /// </summary>
        /// <param name="_cInfo">Infos client du joueur</param>
        /// <param name="_respawnReason">Type de respawn</param>
        /// <param name="_pos">Position du joueur</param>
        private void PlayerSpawnedInWorld(ClientInfo _cInfo, RespawnType _respawnReason, Vector3i _pos)
        {
            try
            {
                if (_cInfo != null && _cInfo.PlatformId != null)
                {
                    string platformId = _cInfo.PlatformId.ToString();
                    if (_respawnReason.Equals(RespawnType.JoinMultiplayer))
                    {
                        ModEventsDiscordBehaviour.PlayerSpawnedInWorld(_cInfo);
                        PersistentContainer.DataChange = true;
                        if (PersistentContainer.Instance.Players[platformId].TPPositions.Count > ChatCommandTP.TPMaxCount)
                        {
                            MSNUtils.LogWarning($"Reset des TP persos pour {_cInfo.playerName}");
                            PersistentContainer.Instance.Players[platformId].TPPositions = new Dictionary<string, string>();
                            PersistentContainer.DataChange = true;
                        }
                    }
                    else if (_respawnReason.Equals(RespawnType.EnterMultiplayer))
                    {
                        PersistentContainer.Instance.Players[platformId].PlayerName = _cInfo.playerName;
                        PersistentContainer.Instance.Players[platformId].Language = MSNLocalization.Language.French;
                        PersistentContainer.Instance.Players[platformId].PlayerWallet = 0;
                        PersistentContainer.Instance.Players[platformId].Time = DateTime.UtcNow;
                        PersistentContainer.Instance.Players[platformId].LastVote = new DateTime();
                        PersistentContainer.Instance.Players[platformId].TPPositions = new Dictionary<string, string>();
                        PersistentContainer.DataChange = true;
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.PlayerSpawnedInWorld: {e.Message}");
            }
        }

        /// <summary>
        /// Méthode s'exécutant lorsqu'un joueur se déconnecte.
        /// </summary>
        /// <param name="_cInfo">Infos client du joueur</param>
        /// <param name="_bShutdown">Est-ce que le serveur s'éteint ?</param>
        private void PlayerDisconnected(ClientInfo _cInfo, bool _bShutdown)
        {
            try
            {
                if (!_bShutdown) ModEventsDiscordBehaviour.PlayerDisconnected(_cInfo);
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in API.PlayerDisconnected: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Méthode s'exécutant à la reception d'un message.
        /// </summary>
        /// <param name="_cInfo">Infos client du joueur</param>
        /// <param name="_type">Type de chat</param>
        /// <param name="_senderId">ID de l'entité</param>
        /// <param name="_msg">Message</param>
        /// <param name="_mainName">Qui envoi le message</param>
        /// <param name="_localizeMain">?</param>
        /// <param name="_recipientEntityIds">?</param>
        /// <returns><see cref="bool"/></returns>
        private bool ChatMessage(ClientInfo _cInfo, EChatType _type, int _senderId, string _msg, string _mainName, bool _localizeMain, List<int> _recipientEntityIds)
        {
            try
            {
                if (_senderId == -1)
                    return true;

                if (ChatCommandTPToServerLocations.Exec(_cInfo, _msg)) return false;
                
                if (_msg.StartsWith(ChatCommandsHook.ChatCommandsPrefix) && ChatCommandsHook.ChatCommandsEnabled)
                {
                    ChatCommandsHook.Exec(_cInfo, _msg);
                    return false;
                }
                else ModEventsDiscordBehaviour.ChatMessage(_cInfo, _type, _msg);
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