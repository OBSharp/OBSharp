using OBSharp.Interop.OBS;

namespace OBSharp.Loader;

public static class Logger
{
    public static void Debug(string text) => Log(400, text);
    public static void Info(string text) => Log(300, text);
    public static void Warn(string text) => Log(200, text);
    public static void Error(string text) => Log(100, text);

    private static void Log(int logLevel, string text) => NativeMethods.Blog(logLevel, $"[OBSharp.Loader]: {text}");
}
