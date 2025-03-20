namespace henryjs.Nuke.Components;

public interface ICompile : IClean, IHasMainProject
{
    Target Compile => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .EnableNoLogo()
                .SetConfiguration(Configuration)
                .SetProjectFile(Solution.Path)
            );
        });
}
