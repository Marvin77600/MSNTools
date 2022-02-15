using MSNTools;
using Logger = Log;

public class MSNUtils
{
    public static void LogWarning(string str)
    {
        Logger.Warning(Config.ModPrefix + " " + str);
    }

    public static void LogError(string str)
    {
        Logger.Error(Config.ModPrefix + " " + str);
    }

    public static void Log(string str)
    {
        Logger.Out(Config.ModPrefix + " " + str);
    }
}