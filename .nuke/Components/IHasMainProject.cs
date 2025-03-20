using henryjs.Nuke.Extensions;

namespace henryjs.Nuke.Components;

public interface IHasMainProject : IHasSolution
{
    /// <summary>
    /// Name of the MainProject (default: <seealso cref="Solution.Name"/>)
    /// </summary>
    [Parameter]
    string MainName => TryGetValue(() => MainName);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    Configuration Configuration => IsLocalBuild ? Configuration.Debug : Configuration.Release;

    /// <summary>
    /// MainProject (default: <seealso cref="MainName"/>)
    /// </summary>
    public Project MainProject => Solution.GetOtherProject(MainName);

    /// <summary>
    /// MainProject (default: <seealso cref="MainName"/>)
    /// </summary>
    /// <returns></returns>
    public Project GetMainProject() => MainProject;
}
