using Nuke.Common.Tooling;

partial class Build
{
    [NuGetPackage(
        packageId: "vpk",
        packageExecutable: "vpk.dll",
        Framework = "net8.0",
        Version = "0.0.831-gde49887"
    )]
    readonly Tool Vpk;
}
