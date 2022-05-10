using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MSNTools.Tools
{
    public class ClearVehicles
    {
        public static bool IsEnabled = false;

        public static void Start()
        {
            File.Delete(GameIO.GetSaveGameDir() + "/vehicles.dat");
            File.Delete(GameIO.GetSaveGameDir() + "/vehicles.dat.bak");
            MSNUtils.LogWarning("Véhicules supprimés !");
        }
    }
}