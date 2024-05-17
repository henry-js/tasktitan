using Nuke.Common.Tooling;

partial class Build
{
    [NuGetPackage(
        packageId: "vpk",
        packageExecutable: "vpk.dll",
        Framework = "net8.0",
        Version = "0.0.462-gf8acc97"
    )]
    readonly Tool Vpk;
}
