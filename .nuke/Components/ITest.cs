using henryjs.Nuke.Extensions;

using Nuke.Common.Tooling;

namespace henryjs.Nuke.Components;

public interface ITest : ICompile, IHasTest
{
    public Func<DotNetTestSettings, DotNetTestSettings> CustomDotNetTestSettings => null;
    Target Test => _ => _
        .TriggeredBy(Compile)
        .Executes(() =>
        {
            ExecuteTests(TestProjects, CustomDotNetTestSettings);
        });

    public new void ExecuteTests(IEnumerable<Project> testProjects, Func<DotNetTestSettings, DotNetTestSettings> customDotNetTestSettings = null)
    {
        var testCombinations =
        from project in testProjects
        select new { project, };
        DotNetTest(_ => _
            .EnableNoLogo()
            .EnableNoBuild()
            .SetConfiguration(Configuration)
            .CombineWith(testCombinations, (_, v) => _
                    .SetProjectFile(v.project)
            )
        );

    }
}

public interface IHasTest
{
    public void ExecuteTests(IEnumerable<Project> testProjects, Func<DotNetTestSettings, DotNetTestSettings> customDotNetTestSettings = null)
    {
        foreach (var project in testProjects)
        {
            var isTest = project.GetProperty("IsTestProject");
            var configurations = project.GetReleases();
            foreach (var conf in configurations)
            {
                DotNetTest(_ => _
                    .SetProjectFile(project)
                    // .SetVerbosity(DotNetVerbosity.normal)
                    .SetCustomDotNetTestSettings(customDotNetTestSettings)
                    );
            }
        }
    }
}
