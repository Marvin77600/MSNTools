﻿using HarmonyLib;
using MSNTools.PersistentData;
using System;

namespace MSNTools.Harmony
{
    [HarmonyPatch(typeof(EntityAlive), "AwardKill")]
    [HarmonyPatch(new Type[] { typeof(EntityAlive) })]
    public class PatchProcessDamageEntityAlive
    {
        static void Postfix(EntityAlive __instance, EntityAlive killer)
        {
            CustomModEvents.AwardKill.Invoke(__instance, killer);
        }
    }
}