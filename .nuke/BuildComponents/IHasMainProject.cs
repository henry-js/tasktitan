namespace henryjs.Nuke.BuildComponents;

public interface IHasMainProject : IHasSolution
{
    /// <summary>
    /// Name of the MainProject (default: <seealso cref="Solution.Name"/>)
    /// </summary>
    [Parameter]
    string MainName => TryGetValue(() => MainName);


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
