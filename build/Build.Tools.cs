using Nuke.Common.Tooling;

partial class Build
{
    [NuGetPackage(
        packageId: "vpk",
        packageExecutable: "vpk.dll",
        Framework = "net8.0",
        Version = "0.0.474-gb9d73fc"
    )]
    readonly Tool Vpk;
}
