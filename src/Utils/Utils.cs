using MSNTools;
using MSNTools.ChatCommands;
using MSNTools.PersistentData;
using Logger = Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MSNUtils
{
    public static void LogWarning(string str)
    {
        Logger.Warning(Config.ModPrefix + " " + str);
    }

    public static void LogError(string str)
    {
        Logger.Warning(Config.ModPrefix + " " + str);
    }

    public static void Log(string str)
    {
        Logger.Warning(Config.ModPrefix + " " + str);
    }
}