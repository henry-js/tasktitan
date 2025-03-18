using ricaun.Nuke.Extensions;

namespace henryjs.Nuke.BuildComponents;

public interface ITest : ICompile, IRelease, IHasTest
{
    Target Test => _ => _
        .TriggeredBy(Compile)
        .Before(Release)
        .Executes(() =>
        {
            var testProjects = Solution.AllProjects.Where(p => p.GetProperty("IsTestProject") != null);
            TestProjects(testProjects);
        });
}

public interface IHasTest
{
    public void TestProjects(IEnumerable<Project> testProjects, Func<DotNetTestSettings, DotNetTestSettings> customDotNetTestSettings = null)
    {
        var failed = false;

        foreach (var project in testProjects)
        {
            var isTest = project.GetProperty("IsTestProject");
            var configurations = project.GetReleases();
            foreach (var conf in configurations)
            {
                DotNetTasks.DotNetTest(_ => _
                    .SetProjectFile(project)
                    // .SetVerbosity(DotNetVerbosity.normal)
                    .SetCustomDotNetTestSettings(customDotNetTestSettings)
                    );
            }
        }
    }
}
