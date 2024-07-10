using Dapper;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using SqlKata.Compilers;
using SqlKata.Execution;

using TaskTitan.Data;

namespace TaskTitan.Tests.Common.Data;

public class TestDatabaseFixture : IDisposable
{
    public readonly string ConnectionString = @$"DataSource={Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.db")}";
    private readonly object _lock = new();
    private bool _dbInitialized;

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (_dbInitialized) { return; }

            using TaskTitanDbContext context = CreateContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.SaveChanges();

            _dbInitialized = true;
        }
        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Execute(Constants.TasksTable.CreateViewTasksWithRowId);
        }

    }

    public TaskTitanDbContext CreateContext()
        => new(
            new DbContextOptionsBuilder<TaskTitanDbContext>()
                .UseSqlite(ConnectionString)
                .Options);

    public void Dispose()
    {
        CreateContext().Database.EnsureDeleted();
        _dbInitialized = false;
    }

    public QueryFactory CreateQueryFactory() => new(new SqliteConnection(ConnectionString), new SqliteCompiler());
}
