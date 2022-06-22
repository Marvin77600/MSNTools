using HarmonyLib;
using AllocsFixes.NetConnections.Servers.Web.Handlers;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace MSNTools.Harmony
{
    public class H_ItemIconHandler
    {
        [HarmonyPatch(typeof(ItemIconHandler), "LoadIcons")]
        public class H_ItemIconHandler_LoadIcons
        {
            static readonly MethodInfo loadIconsFromFolder = AccessTools.Method(typeof(ItemIconHandler), "loadIconsFromFolder");

            static bool Prefix(ItemIconHandler __instance, ref bool ___loaded, ref Dictionary<string, byte[]> ___icons)
            {
                lock (___icons)
                {
                    if (___loaded)
                        return false;
                    MicroStopwatch microStopwatch = new MicroStopwatch();
                    Dictionary<string, List<Color>> _tintedIcons = new Dictionary<string, List<Color>>();
                    foreach (ItemClass itemClass in ItemClass.list)
                    {
                        if (itemClass != null)
                        {
                            Color iconTint = itemClass.GetIconTint();
                            if (iconTint != Color.white)
                            {
                                string iconName = itemClass.GetIconName();
                                if (!_tintedIcons.ContainsKey(iconName))
                                    _tintedIcons.Add(iconName, new List<Color>());
                                _tintedIcons[iconName].Add(iconTint);
                            }
                        }
                    }
                    try
                    {
                        loadIconsFromFolder.Invoke(__instance, new object[] { GameIO.GetGameDir("Data/ItemIcons"), _tintedIcons });
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Failed loading icons from base game");
                        Log.Exception(ex);
                    }
                    foreach (Mod loadedMod in ModManager.GetLoadedMods())
                    {
                        try
                        {
                            loadIconsFromFolder.Invoke(__instance, new object[] { loadedMod.Path + "/UIAtlases/ItemIconAtlas", _tintedIcons });
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Failed loading icons from mod " + loadedMod.ModInfo.Name.Value);
                            Log.Exception(ex);
                        }
                    }
                    ___loaded = true;
                    Log.Out("Web:IconHandler: Icons loaded - {0} ms", (object)microStopwatch.ElapsedMilliseconds);
                    return false;
                }
            }
        }
    }
}