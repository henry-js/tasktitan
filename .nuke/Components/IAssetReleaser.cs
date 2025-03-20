namespace henryjs.Nuke.Components;

public interface IAssetReleaser : IHasAssetReleaser
{
    void ReleaseAssets(string version, string publishDirectory, string releaseDirectory);
}
