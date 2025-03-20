namespace henryjs.Nuke.Extensions;

public static class BuildExtensions
{
    private const string CONFIGURATION_DEBUG = "Debug";

    /// <summary>
    /// Deletes all the bin/obj folders except for the ones in the specified build project directory.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="buildProjectDirectory">The build project directory.</param>
    internal static void CleanSolution(this Solution solution, AbsolutePath buildProjectDirectory)
    {
        var dirs = Globbing.GlobDirectories(solution.Directory, "**/bin", "**/obj")
            .Where(x => !PathConstruction.IsDescendantPath(buildProjectDirectory, x));
        Log.Information("Cleaning {count} directories", dirs.Count());
        foreach (var dir in dirs)
        {
            Log.Information("Directory: {directory}", dir);
        }
        dirs.DeleteFiles();
        solution.GetAllProjects("*");
    }

    /// <summary>
    /// Get MainProject
    /// </summary>
    /// <param name="hasMainProject"></param>
    /// <returns></returns>
    internal static Project GetOtherProject(this Solution solution, string projectName)
        => solution.GetAllProjects("*")
            .FirstOrDefault(p => p.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Gets the release configurations for the specified project.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <returns>The release configurations.</returns>
    public static IEnumerable<string> GetReleases(this Project project)
    {
        return project.GetConfigurations(CONFIGURATION_DEBUG, true);
    }

    /// <summary>
    /// Gets the configurations for the specified project.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <param name="contain">The string to contain in the configuration.</param>
    /// <param name="notContain">A flag indicating whether the configuration should not contain the specified string.</param>
    /// <returns>The configurations.</returns>
    public static IEnumerable<string> GetConfigurations(this Project project, string contain, bool notContain = false)
    {
        var configurations = project.GetConfigurations()
            .Where(s => s.Contains(contain, StringComparison.OrdinalIgnoreCase) != notContain);
        return configurations;
    }

    /// <summary>
    /// Gets the configurations for the specified project.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <returns>The configurations.</returns>
    public static IEnumerable<string> GetConfigurations(this Project project)
    {
        var configurations = project.Configurations
                .Select(pair => pair.Value.Split("|").First())
                .Distinct();
        return configurations;
    }
}
