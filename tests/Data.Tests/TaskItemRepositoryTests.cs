using FluentAssertions;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

using SqlKata.Compilers;
using SqlKata.Execution;

using TaskTitan.Core;
using TaskTitan.Core.Enums;
using TaskTitan.Core.Expressions;
using TaskTitan.Core.OperationResults;
using TaskTitan.Core.Queries;
using TaskTitan.Data.Repositories;
using TaskTitan.Tests.Common.Data;

namespace TaskTitan.Data.Tests;

public class TaskItemRepositoryTests : IClassFixture<TestDatabaseFixture>, IDisposable
{
    private readonly TestDatabaseFixture _fixture;
    private readonly NullLogger<TaskItemRepository> _nullLogger = new();

    public TaskItemRepositoryTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    // [Fact]
    // public async Task AddTaskShouldReturnSuccessResult()
    // {
    //     // Arrange

    //     ITaskItemRepository sut = new TaskItemRepository(_fixture.ConnectionString);
    //     var task = TaskItem.CreateNew("New task for repository layer");

    //     // Act
    //     var result = await sut.AddAsync(task);

    //     // Assert
    //     result.IsSuccess.Should().BeTrue();
    // }

    [Fact]
    public async Task AddShouldAddTaskAndReturnCount()
    {
        // Arrange
        using var dbContext = _fixture.CreateContext();
        using var dbConnection = new SqliteConnection(_fixture.ConnectionString);
        using var db = new QueryFactory(dbConnection, new SqliteCompiler());
        ITaskItemRepository sut = new TaskItemRepository(db, _nullLogger);

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
        using var dbContext = _fixture.CreateContext();
        using var dbConnection = new SqliteConnection(_fixture.ConnectionString);
        var db = new QueryFactory(dbConnection, new SqliteCompiler());

        ITaskItemRepository sut = new TaskItemRepository(db, _nullLogger);

        var tasks = FakeTaskItem.Generate(10);
        dbContext.Tasks.AddRange(tasks);
        dbContext.SaveChanges();

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
        using var dbContext = _fixture.CreateContext();
        using var dbConnection = new SqliteConnection(_fixture.ConnectionString);
        var db = new QueryFactory(dbConnection, new SqliteCompiler());
        ITaskItemRepository sut = new TaskItemRepository(db, _nullLogger);
        var newTask = TaskItem.CreateNew("Test Delete Task");
        var id = newTask.Id;
        dbContext.Tasks.Add(newTask);
        dbContext.SaveChanges();

        // When
        var fetchedTask = sut.GetByFilterAsync([]);
        fetchedTask.Should().NotBeNull();
        await sut.DeleteAsync(newTask);

        // Then
        var deletedTask = (await sut.GetAllAsync()).SingleOrDefault(t => t.Id == id);
        deletedTask.Should().BeNull();
    }

    [Fact]
    public async Task UpdateShouldUpdateAndReturnSuccessResult2()
    {
        // Given
        using var dbContext = _fixture.CreateContext();
        using var dbConnection = new SqliteConnection(_fixture.ConnectionString);
        var db = new QueryFactory(dbConnection, new SqliteCompiler());
        ITaskItemRepository sut = new TaskItemRepository(db, _nullLogger);
        var newTask = TaskItem.CreateNew("Task to update 2");
        string queryFilter = $"Id = '{newTask.Id}'";
        dbContext.Tasks.Add(newTask);
        dbContext.SaveChanges();
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
        var compiler = new SqliteCompiler();
        var db = new QueryFactory(new SqliteConnection(_fixture.ConnectionString), compiler);
        db.Query("tasks").Delete();
    }
}
