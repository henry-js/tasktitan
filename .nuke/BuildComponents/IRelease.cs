namespace henryjs.Nuke.BuildComponents;

public interface IRelease : IHasRelease
{
    Target Release => _ => _
        .Executes(() =>
        {
            ReleaseProject(MainProject);
        });

    public void ReleaseProject(Project project)
    {

    }
}
public interface IHasRelease : IHasMainProject
{
    [Parameter]
    string ReleaseFolderName => TryGetValue(() => ReleaseFolderName) ?? "release";

    AbsolutePath ReleaseDirectory => Solution.Directory / ReleaseFolderName;
}