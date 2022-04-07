using HarmonyLib;
using System;

namespace MSNTools.Harmony
{
    public class H_GameManager
    {
        [HarmonyPatch(typeof(GameManager), "GameMessage", new Type[] { typeof(EnumGameMessages), typeof(string), typeof(EntityAlive), typeof(EntityAlive) })]
        public class H_GameManager_GameMessage
        {
            static bool Prefix(EnumGameMessages _type, EntityAlive _mainEntity, EntityAlive _otherEntity)
            {
                if (_type is EnumGameMessages.EntityWasKilled)
                {
                    CustomModEvents.PlayerKillPlayer.Invoke(_mainEntity, _otherEntity);
                }
                return true;
            }
        }
    }
}