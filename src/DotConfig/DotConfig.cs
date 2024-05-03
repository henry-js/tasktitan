using Dir = System.IO.Directory;
using Serilog;

namespace DotConfig;

public static class DotConfig
{
    private readonly static DirectoryInfo _directory = new(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config"));
    public static bool Exists => _directory.Exists;
    public static void Init(ILogger? logger)
    {
        if (!Exists)
        {
            logger?.Debug(".config directory does not exist");
            _directory.Create();
            logger?.Debug("directory created.");
        }
        else
        {
            logger?.Debug(".config directory already exists.");
        }
    }

    public static bool ChildExists(string configFolder)
    {
        configFolder = Path.GetDirectoryName(configFolder)!;
        var path = Path.Combine(_directory.FullName, configFolder);

        return Dir.Exists(path);
    }

    public static void CreateChild(string configFolder, ILogger? logger = null)
    {
        configFolder = Path.GetDirectoryName(configFolder)!;
        if (!Exists)
        {
            logger?.Debug("{0} directory does not exist", configFolder);
            _directory.Create();
            logger?.Debug("directory created.");
        }
        else
        {
            logger?.Debug("{0} directory already exists.", configFolder);
        }
    }
}
