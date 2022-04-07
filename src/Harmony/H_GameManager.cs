using HarmonyLib;
using System;

namespace MSNTools.Harmony
{
    public class H_GameManager
    {
        [HarmonyPatch(typeof(GameManager), "GameMessage", new Type[] { typeof(EnumGameMessages), typeof(string), typeof(EntityAlive), typeof(EntityAlive) })]
        public class H_GameManager_GameMessage
        {
            static void Postfix(EnumGameMessages _type, EntityAlive _mainEntity, EntityAlive _otherEntity)
            {
                if (_type is EnumGameMessages.EntityWasKilled)
                {
                    if (_mainEntity != null && _otherEntity != null)
                    {
                        if (Zones.IsInTraderArea(_mainEntity as EntityPlayer) && PlayerInvulnerabilityAtTrader.IsEnabled)
                        {
                            ClientInfo killerClientInfo, targetClientInfo;
                            killerClientInfo = PersistentOperations.GetClientInfoFromEntityId(_otherEntity.entityId);
                            targetClientInfo = PersistentOperations.GetClientInfoFromEntityId(_mainEntity.entityId);
                            Discord.DiscordWebhookSender.SendAlertPlayerKilledAtTrader(killerClientInfo, targetClientInfo);
                        }
                    }
                }
            }
        }
    }
}