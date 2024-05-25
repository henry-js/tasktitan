using Microsoft.EntityFrameworkCore;

using TaskTitan.Data;

namespace TaskTitan.Tests.Common.Data;

public class TestDatabaseFixture
{
    private readonly string ConnectionString = @$"DataSource={Path.Combine(Path.GetTempPath(), "test.db")}";
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
    }

    public TaskTitanDbContext CreateContext()
        => new(
            new DbContextOptionsBuilder<TaskTitanDbContext>()
                .UseSqlite(ConnectionString)
                .Options);
}
