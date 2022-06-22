using AllocsFixes.NetConnections.Servers.Web.API;
using AllocsFixes.NetConnections.Servers.Web.Handlers;
using HarmonyLib;
using System;
using System.Reflection;

namespace MSNTools.Harmony
{
    public class H_ApiHandler
    {
        private static MethodInfo addApi = AccessTools.Method(typeof(ApiHandler), "addApi", new Type[] { typeof(WebAPI) });

        [HarmonyPatch(typeof(ApiHandler), MethodType.Constructor, new Type[] { typeof(string) })]
        public class H_ApiHandler_ApiHandler
        {
            static void Postfix(ApiHandler __instance)
            {
                foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (!type.IsAbstract && type.IsSubclassOf(typeof(WebAPI)))
                    {
                        MSNUtils.Log($"Add WebAPI handler : " + type.Name);
                        ConstructorInfo constructor = type.GetConstructor(new Type[0]);
                        if (constructor != null)
                            addApi.Invoke(__instance, new object[] { constructor.Invoke(new object[0]) });
                    }
                }
            }
        }
    }
}