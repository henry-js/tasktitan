using Nuke.Common.Utilities.Collections;

public interface ICompile : INukeBuild, IHasMainProject
{
    Target Compile => _ => _
        .DependsOn<IClean>()
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .EnableNoLogo()
            // .SetConfiguration(Configuration)
            .SetProjectFile(MainProject.Path)
            );
        });
}
public interface IHasSolution : INukeBuild
{
    [Required]
    [Solution(SuppressBuildProjectCheck = true)]
    Solution Solution => TryGetValue(() => Solution);

}

public interface IClean : IHasSolution
{
    Target Clean => _ => _
        .Executes(() =>
        {
            Solution.CleanSolution(BuildProjectDirectory);
        });
}

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

public static class BuildExtensions
{
    /// <summary>
    /// Deletes all the bin/obj folders except for the ones in the specified build project directory.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="buildProjectDirectory">The build project directory.</param>
    public static void CleanSolution(this Solution solution, AbsolutePath buildProjectDirectory)
    {
        var dirs = Globbing.GlobDirectories(solution.Directory, "**/bin", "**/obj")
            .Where(x => !PathConstruction.IsDescendantPath(buildProjectDirectory, x));
        Log.Information("Deleting {count} directories", dirs.Count());
        dirs.DeleteDirectories();
        solution.GetAllProjects("*");
    }

    /// <summary>
    /// Get MainProject
    /// </summary>
    /// <param name="hasMainProject"></param>
    /// <returns></returns>
    public static Project GetOtherProject(this Solution solution, string projectName)
        => solution.GetAllProjects("*")
            .FirstOrDefault(p => p.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase));
}