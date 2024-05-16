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

partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    [Solution(GenerateProjects = true)] readonly Solution Solution;
    [MinVer] MinVer MinVer;
    AbsolutePath ProjectDirectory => SourceDirectory / "Cli";
    AbsolutePath ArtifactsDirectory => RootDirectory / ".artifacts";
    AbsolutePath PublishDirectory => RootDirectory / "publish";
    AbsolutePath ReleaseDirectory => RootDirectory / "release";
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestResultsDirectory => RootDirectory / "testresults";
    IEnumerable<string> Projects => Solution.AllProjects.Select(x => x.Name);

    Target PrintVersion => _ => _
        .Before(Publish)
        .Executes(() =>
        {
            MinVer = MinVerTasks.MinVer(_ => _
                // .SetMinimumMajorMinor("1.0")
                .SetDefaultPreReleasePhase("preview")
                .SetTagPrefix("v")
            ).Result;
            Log.Information("MinVer.Version: {0}", MinVer.Version);
            Log.Information("MinVer.MinverVersion: {0}", MinVer.MinVerVersion);
            Log.Information("Configuration is {0}", Configuration);
        });

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
    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            // Log.Information("Building version {Value}", MinVer.Version);
            DotNetTest(_ => _
                .EnableNoLogo()
                .EnableNoBuild()
                .EnableNoRestore()
                .SetConfiguration(Configuration)
                .SetResultsDirectory(TestResultsDirectory)
                .SetDataCollector("XPlat Code Coverage")
                .SetProjectFile(Solution.Directory)
                .SetRunSetting(
                    "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByAttribute",
                     "Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute")
            );
            var coverageReports = TestResultsDirectory.GetFiles("coverage.cobertura.xml", 2)
                .Select(x => x.ToString());

            if (coverageReports is not null)
            {
                ReportGenerator(_ => _
                    .AddReports(coverageReports)
                    .SetTargetDirectory(RootDirectory / "coveragereport")
                );
            }
            TestResultsDirectory.DeleteDirectory();
        });

    Target Publish => _ => _
                .DependsOn(Compile)
                .Requires(() => Configuration == "Release")
                .Executes(() =>
                {
                    PublishDirectory.CreateOrCleanDirectory();

                    DotNetPublish(_ => _
                        .EnableNoLogo()
                        .EnableNoBuild()
                        .EnableNoRestore()
                        .SetProject(ProjectDirectory)
                        .SetOutput(PublishDirectory)
                        .SetConfiguration(Configuration)
                    );
                    Vpk.Invoke($"pack --packId tasktitan --packVersion {MinVer.Version} --packDir {PublishDirectory} --mainExe task.exe --packTitle tasktitan --outputDir {ReleaseDirectory}");

                    // PublishDirectory.ZipTo(PackDirectory / $"{Solution.Name}.zip", fileMode: FileMode.Create);
                });
}
