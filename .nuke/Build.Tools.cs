using Nuke.Common.Tooling;

partial class Build
{
    [NuGetPackage(
        packageId: "vpk",
        packageExecutable: "vpk.dll",
        Version = "0.0.1053"
    )]
    readonly Tool Vpk;
}
