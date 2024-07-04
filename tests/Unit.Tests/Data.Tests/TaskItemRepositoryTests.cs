using SqlKata.Execution;

using TaskTitan.Core.Enums;
using TaskTitan.Core.Expressions;
using TaskTitan.Core.Queries;
using TaskTitan.Data.Repositories;
using TaskTitan.Tests.Common.Data;

namespace TaskTitan.Data.Tests;

public class TaskItemRepositoryTests : IDisposable
{
    private readonly TestDatabaseFixture _fixture;
    private readonly TaskTitanDbContext _dbContext;
    private readonly QueryFactory _db;
    private readonly NullLogger<TaskItemRepository> _nullLogger = new();

    public TaskItemRepositoryTests()
    {
        _fixture = new TestDatabaseFixture();
        _dbContext = _fixture.CreateContext();
        _db = _fixture.CreateQueryFactory();
    }

    [Fact]
    public async Task AddShouldAddTaskAndReturnCount()
    {
        // Arrange
        ITaskItemRepository sut = new TaskItemRepository(_db, _nullLogger);

        var task = TaskItem.CreateNew("Test Task");

        // Act
        var result = await sut.AddAsync(task);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task GetAllTaskShouldReturnAllWhenTableHasRows()
    {
        // Arrange
        ITaskItemRepository sut = new TaskItemRepository(_db, _nullLogger);

        var tasks = FakeTaskItem.Generate(10);
        _dbContext.Tasks.AddRange(tasks);
        _dbContext.SaveChanges();

        // Act
        var result = await sut.GetByFilterAsync([]);

        // Assert
        result.Should().HaveCount(10);
        result.Should().AllSatisfy(t => t.RowId.Should().NotBe(0), "The database should correctly assign row number");

    }

    [Fact]
    public async Task DeleteShouldDeleteTaskWhenTaskExists()
    {
        // Given
        ITaskItemRepository sut = new TaskItemRepository(_db, _nullLogger);
        var newTask = TaskItem.CreateNew("Test Delete Task");
        var id = newTask.Id;
        _dbContext.Tasks.Add(newTask);
        _dbContext.SaveChanges();

        // When
        var fetchedTask = sut.GetByFilterAsync([]);
        fetchedTask.Should().NotBeNull();
        await sut.DeleteAsync(newTask);

        // Then
        var deletedTask = (await sut.GetAllAsync()).SingleOrDefault(t => t.Id == id);
        deletedTask.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateShouldUpdateAndReturnSuccessResult2()
    {
        // Given
        ITaskItemRepository sut = new TaskItemRepository(_db, _nullLogger);
        var newTask = TaskItem.CreateNew("Task to update 2");
        string queryFilter = $"Id = '{newTask.Id}'";
        _dbContext.Tasks.Add(newTask);
        _dbContext.SaveChanges();
        List<Expression> expressions = [];
        expressions.Add(new IdFilterExpression([new IdRange(1, 5), new IdRange(4, 7)], [9, 5, 99]));
        // When
        DateTime newDate = new(2025, 12, 12);
        Dictionary<string, object?> attributes = [];
        attributes.Add(TaskItemAttribute.Due, newDate.ToString());
        var result = await sut.UpdateByFilter(expressions, attributes);

        // Then
        result.Should().Be(1);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
