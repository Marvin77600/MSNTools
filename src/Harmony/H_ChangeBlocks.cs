using HarmonyLib;
using System;
using System.Collections.Generic;

namespace MSNTools.Harmony
{
    [HarmonyPatch(typeof(GameManager), "ChangeBlocks")]
    [HarmonyPatch(new Type[] { typeof(PlatformUserIdentifierAbs), typeof(List<BlockChangeInfo>) })]
    public class PatchChangeBlocks
    {
        static bool Prefix(GameManager __instance, PlatformUserIdentifierAbs persistentPlayerId, List<BlockChangeInfo> _blocksToChange)
        {
            CustomModEvents.BlockChange.Invoke(__instance, persistentPlayerId, _blocksToChange);
            return true;
        }
    }
}