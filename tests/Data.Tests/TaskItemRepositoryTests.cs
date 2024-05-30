using FluentAssertions;

using Microsoft.Data.Sqlite;

using TaskTitan.Core;
using TaskTitan.Data.Repositories;
using TaskTitan.Tests.Common.Data;

namespace TaskTitan.Data.Tests;

public class TaskItemRepositoryTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

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
    public async Task GetAllTaskShouldReturnAllWhenTableHasRows()
    {
        // Arrange
        using var dbContext = _fixture.CreateContext();
        await dbContext.Tasks.AddRangeAsync(FakeTaskItem.Generate(10));
        await dbContext.SaveChangesAsync();

        using var dbConnection = new SqliteConnection(_fixture.ConnectionString);
        ITaskItemRepository sut = new TaskItemRepository(dbContext, dbConnection);

        // Act
        var result = await sut.GetAllAsync();

        // Assert
        result.Should().HaveCountGreaterThan(0);
    }
}
