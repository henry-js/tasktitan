using henryjs.Nuke.Extensions;

using Nuke.Common.Tooling;

using ricaun.Nuke.Extensions;

namespace henryjs.Nuke.Components;

public interface IMinVer : IHasMinVer
{
    public MinVer GetMinVerVersion(Func<MinVerSettings, MinVerSettings> customMinVerSettings = null)
    {
        var project = MainProject;
        var version = project.GetInformationalVersion();

        return MinVerTasks.MinVer(_ => _
            .SetDefaultPreReleaseIdentifiers("preview")
            .SetProcessOutputLogging(false)
            .SetCustomMinVerSettings(customMinVerSettings)
        ).Result;
    }
}
