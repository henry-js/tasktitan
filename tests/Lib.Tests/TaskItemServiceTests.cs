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
    public async Task AddShouldAddTaskAndReturnCount()
    {
        // Arrange
        using var dbContext = _fixture.CreateContext();
        using var dbConnection = new SqliteConnection(_fixture.ConnectionString);
        ITaskItemRepository repository = new TaskItemRepository(dbContext, dbConnection);

        var task = TaskItem.CreateNew("Test Task");
        var sut = new TaskItemService(repository, dbContext, _nullLogger);

        // Act
        var result = await sut.Add(task);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task DeleteShouldDeleteTaskWhenTaskExists()
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
        var fetchedTask = await sut.Find(id);
        fetchedTask.Should().NotBeNull();
        await sut.Delete(newTask);

        // Then
        var deletedTask = (await sut.GetTasks()).SingleOrDefault(t => t.Id == id);
        deletedTask.Should().BeNull();
    }

    [Fact]
    public async Task GetTasksShouldReturnAllTasks()
    {
        // Arrange
        using var dbContext = _fixture.CreateContext();
        using var dbConnection = new SqliteConnection(_fixture.ConnectionString);
        ITaskItemRepository repository = new TaskItemRepository(dbContext, dbConnection);
        dbContext.Tasks.AddRange(FakeTaskItem.Generate(2));
        dbContext.SaveChanges();
        ITaskItemService sut = new TaskItemService(repository, dbContext, _nullLogger);

        // Act
        var tasks = await sut.GetTasks();

        // Assert
        tasks.Should().HaveCount(2);
        tasks.Should().AllSatisfy(t => t.RowId.Should().NotBe(0), "The database should correctly assign row number");
    }

    [Fact]
    public async Task UpdateShouldUpdateAndReturnSuccessResult()
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
        TaskItem taskToUpdate = await sut.Find(id) ?? throw new Exception();
        DateTime newDate = new(2025, 12, 12);
        taskToUpdate.Due = newDate;
        var result = await sut.Update(taskToUpdate);

        // Then
        result.IsSuccess.Should().Be(true);
        result.ErrorMessages.Should().BeEmpty();
    }
}
