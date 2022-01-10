using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MSNTools
{
    public abstract class CustomModEventAbs<TDelegate>
    {
        public string eventName;
        protected readonly List<Receiver> receivers = new List<Receiver>();

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void RegisterHandler(TDelegate _handlerFunc)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            bool _coreGame = false;
            Mod _mod = null;
            if (callingAssembly.Equals(executingAssembly))
            {
                _coreGame = true;
            }
            else
            {
                _mod = ModManager.GetModForAssembly(callingAssembly);
                if (_mod == null)
                    Log.Warning("[MODS] Could not find mod that tries to register a handler for event " + eventName);
            }
            receivers.Add(new Receiver(_mod, _handlerFunc, _coreGame));
        }

        public void UnregisterHandler(TDelegate _handlerFunc)
        {
            for (int index = 0; index < receivers.Count; ++index)
            {
                if (receivers[index].DelegateFunc.Equals(_handlerFunc))
                    receivers.RemoveAt(index);
            }
        }

        protected void LogError(Exception _e, Receiver _currentMod)
        {
            Log.Error($"[MODS] Error while executing {eventName} on mod \"{_currentMod.ModName}\"");
            Log.Exception(_e);
        }

        protected class Receiver
        {
            public readonly Mod Mod;
            public readonly TDelegate DelegateFunc;
            private readonly bool coreGame;

            public Receiver(Mod _mod, TDelegate _handler, bool _coreGame = false)
            {
                Mod = _mod;
                DelegateFunc = _handler;
                coreGame = _coreGame;
            }

            public string ModName
            {
                get
                {
                    if (Mod != null)
                        return Mod.ModInfo.Name.Value;
                    return coreGame ? "-GameCore-" : "-UnknownMod-";
                }
            }
        }
    }
}