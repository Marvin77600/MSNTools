using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace MSNTools.Harmony
{
    public class H_VehicleManager
    {
        public static List<EntityVehicle> vehiclesActive = new List<EntityVehicle>();
        public static List<EntityCreationData> vehiclesUnloaded = new List<EntityCreationData>();

        [HarmonyPatch(typeof(VehicleManager), "Update")]
        public class H_VehicleManager_Update
        {
            static void Postfix(List<EntityVehicle> ___vehiclesActive, List<EntityCreationData> ___vehiclesUnloaded)
            {
                vehiclesActive = ___vehiclesActive;
                vehiclesUnloaded = ___vehiclesUnloaded;
            }
        }
    }
}