using HarmonyLib;
using System;

namespace MSNTools.Harmony
{
    [HarmonyPatch(typeof(EntityAlive), "AwardKill", new Type[] { typeof(EntityAlive) })]
    public class PatchProcessDamageEntityAlive
    {
        static void Postfix(EntityAlive __instance, EntityAlive killer)
        {
            CustomModEvents.AwardKill.Invoke(__instance, killer);
        }
    }
}