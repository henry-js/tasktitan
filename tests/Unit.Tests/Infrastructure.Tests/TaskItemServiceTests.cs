using Microsoft.Data.Sqlite;

using SqlKata.Compilers;
using SqlKata.Execution;

using TaskTitan.Data.Repositories;
using TaskTitan.Infrastructure.Expressions;
using TaskTitan.Tests.Common.Data;

using Xunit.Categories;

namespace TaskTitan.Infrastructure.Tests;

[UnitTest]
public class TaskItemServiceTests : IDisposable
{
    private readonly TestDatabaseFixture _fixture;
    private readonly TaskTitanDbContext _dbContext;
    private readonly NullLogger<TaskItemService> _serviceLogger;
    private readonly NullLogger<TaskItemRepository> _repoLogger;
    private readonly IExpressionParser _parser;
    private readonly DateTimeConverter _stringConverter;
    private readonly QueryFactory _db;

    public TaskItemServiceTests()
    {
        _fixture = new TestDatabaseFixture();
        _dbContext = _fixture.CreateContext();
        _db = _fixture.CreateQueryFactory();
        _serviceLogger = new();
        _repoLogger = new();
        _parser = new ExpressionParser();
        _stringConverter = new DateTimeConverter(TimeProvider.System);
    }

    [Fact]
    public async Task AddShouldAddTaskAndReturnCount()
    {
        // Arrange
        ITaskItemRepository repository = new TaskItemRepository(_db, _repoLogger);
        var request = new TaskItemCreateRequest() { NewTask = new() { Description = "Test Task" } };
        var sut = new TaskItemService(repository, _parser, _stringConverter, _serviceLogger);

        // Act
        var result = await sut.Add(request);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task DeleteShouldDeleteTaskWhenTaskExists()
    {
        // Given
        ITaskItemRepository repository = new TaskItemRepository(_db, _repoLogger);
        ITaskItemService sut = new TaskItemService(repository, _parser, _stringConverter, _serviceLogger);
        var newTask = TaskItem.CreateNew("Test Delete Task");
        var id = newTask.Id;
        _dbContext.Tasks.Add(newTask);
        _dbContext.SaveChanges();

        // When
        var fetchedTask = await sut.GetTasks([]);
        fetchedTask.Should().NotBeNull();
        await sut.Delete(newTask);

        // Then
        var deletedTask = (await sut.GetTasks([])).SingleOrDefault(t => t.Id == id);
        deletedTask.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTasksShouldReturnAllTasks()
    {
        // Arrange
        // using var dbContext = _fixture.CreateContext();
        ITaskItemRepository repository = new TaskItemRepository(_db, _repoLogger);
        _dbContext.Tasks.AddRange(FakeTaskItem.Generate(2));
        _dbContext.SaveChanges();
        ITaskItemService sut = new TaskItemService(repository, _parser, _stringConverter, _serviceLogger);

        // Act
        var tasks = await sut.GetTasks([]);

        // Assert
        tasks.Should().HaveCount(2);
        tasks.Should().AllSatisfy(t => t.RowId.Should().NotBe(0), "The database should correctly assign row number");
    }

    [Fact]
    public async Task UpdateShouldUpdateAndReturnSuccessResult()
    {
        // Given
        // using var dbContext = _fixture.CreateContext();
        ITaskItemRepository repository = new TaskItemRepository(_db, _repoLogger);
        var newTask = TaskItem.CreateNew("Task to update");
        var id = newTask.Id;
        _dbContext.Tasks.Add(newTask);
        _dbContext.SaveChanges();
        ITaskItemService sut = new TaskItemService(repository, _parser, _stringConverter, _serviceLogger);

        // When
        DateTime newDate = new(2025, 12, 12);
        TaskItemModifyRequest modifyRequest = new();
        modifyRequest.Attributes.Add(Core.Enums.TaskItemAttribute.Due, newDate.ToString("yyyy-MM-dd"));
        var result = await sut.Update(modifyRequest);

        // Then
        result.Should().Be(1);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
