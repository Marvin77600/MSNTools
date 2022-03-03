using MSNTools.Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSNTools.Discord
{
    public class BloodMoonAlerts
    {
        public static bool IsEnabled = true;
        public static int Hour = 0;
        public static bool AlertAlreadySent = false;

        public static bool BloodMoonStartSoon()
        {
            try
            {
                int day = GameUtils.WorldTimeToDays(GameManager.Instance.World.GetWorldTime());
                int bloodMoonFrequency = GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency);
                int modulo = day % bloodMoonFrequency;

                if (modulo == 0)
                {
                    ulong worldTime = GameManager.Instance.World.worldTime;
                    int num = GameStats.GetInt(EnumGameStats.BloodMoonWarning);
                    (int Days, int Hours, int Minutes) = GameUtils.WorldTimeToElements(worldTime);
                    if (Hours >= Hour && Minutes >= 0 && !AlertAlreadySent)
                    {
                        DiscordWebhookSender.SendBloodMoonAlert();
                        AlertAlreadySent = true;
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in BloodMoonAlerts.BloodMoonStartSoon: {e.Message}");
            }
            return false;
        }
    }
}
