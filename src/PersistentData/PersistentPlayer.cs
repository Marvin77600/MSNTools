﻿using System;
using System.Collections.Generic;

namespace MSNTools.PersistentData
{
    [Serializable]
    public class PersistentPlayer
    {
        public readonly string platformIdentifierString;
        private MSNLocalization.Language language;
        private int playerWallet;
        private string playerName;
        private DateTime lastVote;
        private bool isDonator;
        private DateTime time;
        private Dictionary<string, string> tpPositions;

        public MSNLocalization.Language Language
        {
            get
            {
                return language;
            }
            set 
            {
                language = value;
            }
        }

        public DateTime LastVote
        {
            get
            {
                return lastVote;
            }
            set
            {
                lastVote = value;
            }
        }

        public DateTime Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }

        public bool IsDonator
        {
            get
            {
                return isDonator;
            }
            set
            {
                isDonator = value;
            }
        }

        public Dictionary<string, string> TPPositions
        {
            get
            {
                return tpPositions;
            }
            set
            {
                tpPositions = value;
            }
        }

        public string PlayerName
        {
            get
            {
                return playerName;
            }
            set
            {
                playerName = value;
            }
        }

        public int PlayerWallet
        {
            get
            {
                return playerWallet;
            }
            set
            {
                playerWallet = value; 
            }
        }

        public PersistentPlayer(string PlatformIdentifierString)
        {
            platformIdentifierString = PlatformIdentifierString;
        }
    }
}