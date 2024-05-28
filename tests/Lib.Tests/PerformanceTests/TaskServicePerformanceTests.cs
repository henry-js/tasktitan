using TaskTitan.Tests.Common.Data;

using Xunit.Categories;

namespace TaskTitan.Lib.Tests;

[Category("Performance")]
public class TaskItemServicePerformanceTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;
    private readonly ITestOutputHelper _outputHelper;


    public TaskItemServicePerformanceTests(TestDatabaseFixture fixture, ITestOutputHelper outputHelper)
    {
        _fixture = fixture;
        _outputHelper = outputHelper;
    }

    [Fact]
    public void FetchingTasksShouldBePerformant()
    {
        using var context = _fixture.CreateContext();

        var fakeTasks = FakeTaskItem.Generate(100000);
        context.AddRange(fakeTasks);
        context.SaveChanges();

        var service = new TaskItemService(context, new NullLogger<TaskItemService>());
        var st = Stopwatch.StartNew();
        var tasks = service.GetTasks();
        st.Stop();
        st.ElapsedMilliseconds.Should().BeLessThan(100);
        tasks.Should().HaveCountGreaterThan(1);
    }
}
