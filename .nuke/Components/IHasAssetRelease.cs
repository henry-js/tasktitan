namespace henryjs.Nuke.Components;

public interface IHasAssetRelease : IHasMainProject
{
    [Parameter]
    string ReleaseFolderName => TryGetValue(() => ReleaseFolderName) ?? "release";
    AbsolutePath ReleaseDirectory => Solution.Directory / ReleaseFolderName;
}
