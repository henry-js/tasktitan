namespace TaskTitan.Cli;

public static class Constants
{
    static Constants()
    {
#if DEBUG
        ConfigurationDirectory = "./";
#else
        ConfigurationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".tasktitan");
#endif
    }
    public static readonly string ConfigurationDirectory;
}
