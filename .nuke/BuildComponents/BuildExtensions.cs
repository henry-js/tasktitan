namespace henryjs.Nuke.BuildComponents;

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
