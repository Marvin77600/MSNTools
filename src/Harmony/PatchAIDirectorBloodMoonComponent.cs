using HarmonyLib;
using System;
using System.Collections.Generic;

namespace MSNTools.Harmony
{
    [HarmonyPatch(typeof(AIDirectorBloodMoonComponent), "StartBloodMoon")]
    public class PatchAIDirectorBloodMoonComponent_StartBloodMoon
    {
        static bool Prefix()
        {
            CustomModEvents.StartBloodMoon.Invoke();
            return true;
        }
    }

    [HarmonyPatch(typeof(AIDirectorBloodMoonComponent), "EndBloodMoon")]
    public class PatchAIDirectorBloodMoonComponent_EndBloodMoon
    {
        static bool Prefix()
        {
            CustomModEvents.EndBloodMoon.Invoke();
            return true;
        }
    }
}