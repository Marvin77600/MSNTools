using System;
using System.Collections.Generic;

namespace MSNTools.PersistentData
{
    [Serializable]
    public class PersistentPlayers
    {
        public Dictionary<string, PersistentPlayer> Players = new Dictionary<string, PersistentPlayer>();

        public PersistentPlayer this[string platformIdentifierString]
        {
            get
            {
                if (string.IsNullOrEmpty(platformIdentifierString))
                {
                    return null;
                }
                else if (Players.ContainsKey(platformIdentifierString))
                {
                    return Players[platformIdentifierString];
                }
                else
                {
                    if (platformIdentifierString != null)
                    {
                        PersistentPlayer p = new PersistentPlayer(platformIdentifierString);
                        Players.Add(platformIdentifierString, p);
                        return p;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}