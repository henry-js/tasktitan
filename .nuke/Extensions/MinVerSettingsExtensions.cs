namespace henryjs.Nuke.Extensions;

public static class MinVerSettingsExtensions
{
    public static MinVerSettings SetCustomMinVerSettings(
        this MinVerSettings minVerSettings,
        Func<MinVerSettings, MinVerSettings> customSettings)
    {
        if (customSettings is null) return minVerSettings;

        return customSettings.Invoke(minVerSettings);
    }

    public static DotNetTestSettings SetCustomDotNetTestSettings(
    this DotNetTestSettings dotnetTestSettings,
    Func<DotNetTestSettings, DotNetTestSettings> customSettings)
    {
        if (customSettings is null) return dotnetTestSettings;

        return customSettings.Invoke(dotnetTestSettings);
    }
}
