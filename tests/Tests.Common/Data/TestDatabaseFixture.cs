using Dapper;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using TaskTitan.Data;

namespace TaskTitan.Tests.Common.Data;

public class TestDatabaseFixture : IDisposable
{
    public readonly string ConnectionString = @$"DataSource={Path.Combine(Path.GetTempPath(), "test.db")}";
    private static readonly object _lock = new();
    private static bool _dbInitialized;

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
            connection.Execute(DbConstants.TasksTable.CreateViewTasksWithRowId);
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
    }
}
