using System.Diagnostics;

using Microsoft.Extensions.Logging.Abstractions;

using TaskTitan.Core;

using TaskTitan.Lib.Services;

using Xunit.Abstractions;

namespace TaskTitan.Lib.Tests;
public class TaskServiceTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;
    private readonly ITestOutputHelper _outputHelper;


    public TaskServiceTests(TestDatabaseFixture fixture, ITestOutputHelper outputHelper)
    {
        _fixture = fixture;
        _outputHelper = outputHelper;

    }

    [Fact]
    public void FetchingTasksShouldBePerformant()
    {
        using var context = _fixture.CreateContext();

        var fakeTasks = FakeTtask.Generate(100000);
        context.AddRange(fakeTasks);
        context.SaveChanges();

        var service = new TaskService(context, new NullLogger<TaskService>());
        var st = Stopwatch.StartNew();
        var tasks = service.GetTasks();
        st.Stop();
        st.ElapsedMilliseconds.Should().BeLessThan(100);
        tasks.Should().HaveCountGreaterThan(1);
    }
}
