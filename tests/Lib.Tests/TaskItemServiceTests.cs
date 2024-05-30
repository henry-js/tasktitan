using Microsoft.Data.Sqlite;

using TaskTitan.Data.Repositories;
using TaskTitan.Tests.Common.Data;

using Xunit.Categories;

namespace TaskTitan.Lib.Tests;

[UnitTest]
public class TaskItemServiceTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;
    private readonly NullLogger<TaskItemService> _nullLogger;

    public TaskItemServiceTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
        _nullLogger = new();
    }

    [Fact]
    public void AddShouldAddTaskAndReturnCount()
    {
        // Arrange
        using var dbContext = _fixture.CreateContext();
        using var dbConnection = new SqliteConnection(_fixture.ConnectionString);
        ITaskItemRepository repository = new TaskItemRepository(dbContext, dbConnection);

        var task = TaskItem.CreateNew("Test Task");
        var sut = new TaskItemService(repository, dbContext, _nullLogger);

        // Act
        var result = sut.Add(task);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void DeleteShouldDeleteTaskWhenTaskExists()
    {
        // Given
        using var dbContext = _fixture.CreateContext();
        using var dbConnection = new SqliteConnection(_fixture.ConnectionString);
        ITaskItemRepository repository = new TaskItemRepository(dbContext, dbConnection);
        ITaskItemService sut = new TaskItemService(repository, dbContext, _nullLogger);
        var newTask = TaskItem.CreateNew("Test Delete Task");
        var id = newTask.Id;
        dbContext.Tasks.Add(newTask);
        dbContext.SaveChanges();

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
        using var dbContext = _fixture.CreateContext();
        using var dbConnection = new SqliteConnection(_fixture.ConnectionString);
        ITaskItemRepository repository = new TaskItemRepository(dbContext, dbConnection);
        dbContext.Tasks.AddRange(FakeTaskItem.Generate(2));
        dbContext.SaveChanges();
        ITaskItemService sut = new TaskItemService(repository, dbContext, _nullLogger);

        // Act
        var tasks = sut.GetTasks();

        // Assert
        tasks.Should().HaveCount(2);
        tasks.Should().AllSatisfy(t => t.RowId.Should().NotBe(0), "The database should correctly assign row number");
    }

    [Fact]
    public void UpdateShouldUpdateAndReturnSuccessResult()
    {
        // Given
        using var dbContext = _fixture.CreateContext();
        using var dbConnection = new SqliteConnection(_fixture.ConnectionString);
        ITaskItemRepository repository = new TaskItemRepository(dbContext, dbConnection);
        var newTask = TaskItem.CreateNew("Task to update");
        var id = newTask.Id;
        dbContext.Tasks.Add(newTask);
        dbContext.SaveChanges();
        ITaskItemService sut = new TaskItemService(repository, dbContext, _nullLogger);

        // When
        TaskItem taskToUpdate = sut.Find(id) ?? throw new Exception();
        DateTime newDate = new(2025, 12, 12);
        taskToUpdate.Due = newDate;
        var result = sut.Update(taskToUpdate);

        // Then
        result.IsSuccess.Should().Be(true);
        result.ErrorMessages.Should().BeEmpty();
    }
}
