using MSNTools;
using Logger = Log;

public class MSNUtils
{
    /// <summary>
    /// Envoi un <see cref="Logger.Warning(string)"/> précedé du <see cref="Config.ModPrefix"/>
    /// </summary>
    /// <param name="obj"></param>
    public static void LogWarning(object obj)
    {
        Logger.Warning(Config.ModPrefix + " " + obj.ToString());
    }

    /// <summary>
    /// Envoi un <see cref="Logger.Error(string)"/> précedé du <see cref="Config.ModPrefix"/>
    /// </summary>
    /// <param name="obj"></param>
    public static void LogError(object obj)
    {
        Logger.Error(Config.ModPrefix + " " + obj.ToString());
    }

    /// <summary>
    /// Envoi un <see cref="Logger.Out(string)"/> précedé du <see cref="Config.ModPrefix"/>
    /// </summary>
    /// <param name="obj"></param>
    public static void Log(object obj)
    {
        Logger.Out(Config.ModPrefix + " " + obj.ToString());
    }
}