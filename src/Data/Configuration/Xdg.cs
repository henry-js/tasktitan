namespace TaskTitan.Configuration;

internal static class Xdg
{
    static Xdg()
    {
        DATA_HOME = Environment.GetEnvironmentVariable("XDG_DATA_HOME", EnvironmentVariableTarget.User);
        CONFIG_HOME = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME", EnvironmentVariableTarget.User);
        STATE_HOME = Environment.GetEnvironmentVariable("XDG_STATE_HOME", EnvironmentVariableTarget.User);
    }

    public static string? DATA_HOME { get; }
    public static string? CONFIG_HOME { get; }
    public static string? STATE_HOME { get; }

}
