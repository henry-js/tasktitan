using System.Collections;

using henryjs.Nuke.Extensions;

namespace henryjs.Nuke.Components;

public interface IHasMainProject : IHasSolution
{
    /// <summary>
    /// Name of the MainProject (default: <seealso cref="Solution.Name"/>)
    /// </summary>
    [Parameter]
    string Project => TryGetValue(() => Project);

    [Parameter]
    string[] Projects => TryGetValue(() => Projects) ?? [Project];

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    Configuration Configuration => IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Runtimes you want to compile & test against - Default is '<empty>'")]
    string[] Runtimes => TryGetValue(() => Runtimes) ?? [""];

    [Parameter("Target framework - Default is 'net9.0'")]
    string[] Frameworks => TryGetValue(() => Frameworks) ?? ["net9.0"];

    /// <summary>
    /// MainProject (default: <seealso cref="Project"/>)
    /// </summary>
    public Project MainProject => Solution.GetOtherProject(Project);

    public IEnumerable<Project> BuildProjects => Projects.Select(n => Solution.GetOtherProject(n));

    /// <summary>
    /// MainProject (default: <seealso cref="Project"/>)
    /// </summary>
    /// <returns></returns>
    public Project GetMainProject() => MainProject;
}
