using HarmonyLib;
using System;
using System.Collections.Generic;

namespace MSNTools.Harmony
{
    public class H_AIDirectorBloodMoonComponent
    {
        [HarmonyPatch(typeof(AIDirectorBloodMoonComponent), "StartBloodMoon")]
        public class H_AIDirectorBloodMoonComponent_StartBloodMoon
        {
            static bool Prefix()
            {
                CustomModEvents.StartBloodMoon.Invoke();
                return true;
            }
        }

        [HarmonyPatch(typeof(AIDirectorBloodMoonComponent), "EndBloodMoon")]
        public class H_AIDirectorBloodMoonComponent_EndBloodMoon
        {
            static bool Prefix()
            {
                CustomModEvents.EndBloodMoon.Invoke();
                return true;
            }
        }
    }
}