namespace Healthcheck.Service.LogParsing
{
    /// <summary>
    /// Log level
    /// </summary>
    public enum LogLevel : byte
    {
        UNKNOWN,
        INFO,
        WARN,
        DEBUG,
        ERROR,
        FATAL,
        SYSTEM,
    }
}