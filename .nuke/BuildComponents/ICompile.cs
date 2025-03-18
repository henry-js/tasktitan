namespace henryjs.Nuke.BuildComponents;

public interface ICompile : INukeBuild, IHasMainProject
{
    Target Compile => _ => _
        .DependsOn<IClean>()
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .EnableNoLogo()
            // .SetConfiguration(Configuration)
            .SetProjectFile(MainProject?.Path ?? Solution.Path)
            );
        });
}

public interface IPublish : IHasMainProject
{
    Target Publish => _ => _
        .DependsOn<ICompile>()
        .Executes(() =>
        {
            DotNetPublish(_ => _
                .EnableNoLogo()
                .SetProject(MainProject));
        });
}