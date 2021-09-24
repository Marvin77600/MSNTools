using System.Collections.Generic;
using System.Timers;

namespace MSNTools
{
    class Timers
    {
        public static bool IsRunning = false;
        private static int CoreCount, _twoSecondTick, _fiveSecondTick, _tenSecondTick, _twentySecondTick, _oneMinTick, _twoMinTick, _fiveMinTick = 0;
        private static readonly System.Timers.Timer Core = new System.Timers.Timer();

        public static void TimerStart()
        {
            if (CoreCount < 1)
            {
                CoreCount++;
                IsRunning = true;
                Core.Interval = 1000;
                Core.Start();
                Core.Elapsed += new ElapsedEventHandler(Tick);
            }
        }

        public static void TimerStop()
        {
            CoreCount = 0;
            IsRunning = false;
            Core.Stop();
            Core.Close();
            Core.Dispose();
        }

        private static void Tick(object sender, ElapsedEventArgs e)
        {
            _twoSecondTick++;
            _fiveSecondTick++;
            _tenSecondTick++;
            _twentySecondTick++;
            _oneMinTick++;
            _twoMinTick++;
            _fiveMinTick++;
            Exec();
        }

        private static void Exec()
        {
            if (PlayerChecks.GodEnabled || PlayerChecks.SpectatorEnabled)
            {
                PlayerChecks.Exec();
            }

            if (_twoSecondTick >= 2)
            {
                _twoSecondTick = 0;
            }
            if (_fiveSecondTick >= 5)
            {
                _fiveSecondTick = 0;
            }
            if (_tenSecondTick >= 10)
            {
                _tenSecondTick = 0;
            }
            if (_twentySecondTick >= 20)
            {
                _twentySecondTick = 0;
            }
            if (_oneMinTick >= 60)
            {
                _oneMinTick = 0;
            }
            if (_twoMinTick >= 120)
            {
                _twoMinTick = 0;
                if (InventoryChecks.Check_Storage)
                {
                    InventoryChecks.CheckStoragesInLoadedChunks();
                }
            }
            if (_fiveMinTick >= 300)
            {
                _fiveMinTick = 0;
            }
        }
    }
}
