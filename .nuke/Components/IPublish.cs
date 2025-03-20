using Nuke.Common.Tooling;

namespace henryjs.Nuke.Components;

public interface IPublish : IHasPublish, ICompile
{
    Target Publish => _ => _
        .DependsOn(Compile)
        .Requires(() => MainName)
        .Executes(() =>
        {
            var result = DotNetPublish(_ => _
                .EnableNoLogo()
                .EnableNoRestore()
                .SetProject(MainProject)
                .SetConfiguration(Configuration)
                .SetOutput(PublishDirectory)
                .SetVerbosity(DotNetVerbosity.minimal)
                .EnableProcessOutputLogging()
                );

            Log.Information("Publish output captured: {captured}", result is not null);
        });
}
public interface IHasPublish : IHasMainProject
{
    [Parameter]
    string PublishFolderName => TryGetValue(() => PublishFolderName) ?? "publish";
    AbsolutePath PublishDirectory => Solution.Directory / PublishFolderName;
}