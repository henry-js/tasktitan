using Nuke.Common.Tooling;

using ricaun.Nuke.Extensions;

namespace henryjs.Nuke.Components;
public interface IAssetRelease : IHasAssetReleaser, IHasAssetRelease, IPublish
{
    Target AssetRelease => _ => _
        .DependsOn(Publish)
        .Executes(() =>
        {
            AssetReleaser.ReleaseAssets(MainProject.GetInformationalVersion(), PublishDirectory, ReleaseDirectory);
        });
}

public class VelopackAssetReleaser : IAssetReleaser
{
    public Tool Vpk { get; set; }
    public VelopackAssetReleaser(Tool Velopack)
    {
        Vpk = Velopack;
    }

    public void ReleaseAssets(string version, string publishDirectory, string releaseDirectory)
    {
        var x = Vpk.Invoke($"pack --packId tasktitan --packVersion {version} --packDir {publishDirectory} --mainExe task.exe --packTitle tasktitan --outputDir {releaseDirectory} --shortcuts None");

    }
}