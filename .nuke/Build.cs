using henryjs.Nuke.Components;

using Nuke.Common.Tooling;

partial class Build : NukeBuild, IAssetRelease, IPublish
{
    [NuGetPackage(
    packageId: "vpk",
    packageExecutable: "vpk.dll",
    Version = "0.0.1053"
    )]
    readonly Tool Vpk;
    [MinVer]
    readonly MinVer MinVer;
    IAssetRelease Release => this;
    Target IAssetRelease.AssetRelease => _ => _
        .DependsOn<IPublish>(x => x.Publish)
        .OnlyWhenDynamic(Release.MainProjectIsExecutable)
        .Executes(() =>
        {
            Log.Information("Cleaning Directory: {Directory}", Release.ReleaseDirectory);
            Release.ReleaseDirectory.CreateOrCleanDirectory();

            var pkgId = Release.PackageId;
            // var pubVer = (this as IHasMainProject).MainProject.GetPublishedVersion((this as IPublish).PublishDirectory);
            var pubDir = Release.PublishDirectory;
            var mainExe = Release.AssetExecutable;
            var relDir = Release.ReleaseDirectory;

            var minver = MinVerTasks.MinVer(_ => _.SetDefaultPreReleaseIdentifiers("preview")).Result;

            Vpk.Invoke($"pack --packId {pkgId} --packVersion {minver.MinVerVersion} --packDir {pubDir} --mainExe {mainExe}.exe --outputDir {relDir} --shortcuts None");
        });
    public static int Main()
    {
        var res = Execute<Build>(x => (x as ICompile).Compile);
        Console.ReadLine();
        return res;
    }
}