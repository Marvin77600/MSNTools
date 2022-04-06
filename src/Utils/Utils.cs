using MSNTools;
using Logger = Log;

public class MSNUtils
{
    public static void LogWarning(object obj)
    {
        Logger.Warning(Config.ModPrefix + " " + obj.ToString());
    }

    public static void LogError(object obj)
    {
        Logger.Error(Config.ModPrefix + " " + obj.ToString());
    }

    public static void Log(object obj)
    {
        Logger.Out(Config.ModPrefix + " " + obj.ToString());
    }
}