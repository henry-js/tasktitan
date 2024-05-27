using TaskTitan.Tests.Common.Data;

using Xunit.Categories;

namespace TaskTitan.Lib.Tests;

[UnitTest]
public class TaskServiceTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;
    private readonly NullLogger<TaskService> _nullLogger;

    public TaskServiceTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
        _nullLogger = new();
    }

    [Fact]
    public void AddShouldAddTaskAndReturnCount()
    {
        // Arrange
        var task = TTask.CreateNew("Test Task");
        using var context = _fixture.CreateContext();
        var sut = new TaskService(context, _nullLogger);

        // Act
        var result = sut.Add(task);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void DeleteShouldDeleteTaskWhenTaskExists()
    {
        // Given
        using var context = _fixture.CreateContext();
        ITtaskService sut = new TaskService(context, _nullLogger);
        var newTask = TTask.CreateNew("Test Delete Task");
        var id = newTask.Id;
        context.Tasks.Add(newTask);
        context.SaveChanges();

        // When
        var fetchedTask = sut.Find(id);
        fetchedTask.Should().NotBeNull();
        sut.Delete(newTask);


        // Then
        var deletedTask = sut.GetTasks().SingleOrDefault(t => t.Id == id);
        deletedTask.Should().BeNull();
    }

    [Fact]
    public void GetTasksShouldReturnAllTasks()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        context.Tasks.AddRange(FakeTtask.Generate(2));
        context.SaveChanges();
        ITtaskService sut = new TaskService(context, _nullLogger);

        // Act
        var tasks = sut.GetTasks();

        // Assert
        tasks.Should().HaveCount(2);
    }

    [Fact]
    public void UpdateShouldUpdateAndReturnSuccessResult()
    {
        // Given
        using var context = _fixture.CreateContext();
        var newTask = TTask.CreateNew("Task to update");
        var id = newTask.Id;
        context.Tasks.Add(newTask);
        context.SaveChanges();
        ITtaskService sut = new TaskService(context, _nullLogger);

        // When
        TTask taskToUpdate = sut.Find(id) ?? throw new Exception();
        DateTime newDate = new(2025, 12, 12);
        taskToUpdate.Due = newDate;
        var result = sut.Update(taskToUpdate);

        // Then
        result.IsSuccess.Should().Be(true);
        result.ErrorMessages.Should().BeEmpty();
    }
}
