using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.MinVer;
using Nuke.Common.Tools.ReportGenerator;

using Serilog;

using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;

class Build : NukeBuild
{

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    [Solution(GenerateProjects = true)] readonly Solution Solution;
    AbsolutePath ProjectDirectory => SourceDirectory / "Cli";
    AbsolutePath ArtifactsDirectory => RootDirectory / ".artifacts";
    AbsolutePath PublishDirectory => RootDirectory / "publish";
    AbsolutePath PackDirectory => RootDirectory / "packages";
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestDirectory => RootDirectory / "tests";
    IEnumerable<string> Projects => Solution.AllProjects.Select(x => x.Name);

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            if (ArtifactsDirectory.Exists())
            {
                ArtifactsDirectory.CreateOrCleanDirectory();
            }
            else
            {
                SourceDirectory.GlobDirectories("**/{obj,bin}")
                               .DeleteDirectories();
            }
        });


    Target Restore => _ => _
    .After(Clean)
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetForce(true)
                .SetProjectFile(Solution.Directory));
        });

    Target Compile => _ => _
        .DependsOn(Clean, Restore)
        .Executes(() =>
        {
            // Log.Information("Building version {Value}", MinVer.Version);
            DotNetBuild(_ => _
                .EnableNoLogo()
                .EnableNoRestore()
                .SetProjectFile(Solution.Directory)
                .SetConfiguration(Configuration)
            );
        });

}
