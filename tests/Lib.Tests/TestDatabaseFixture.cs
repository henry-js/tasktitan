namespace TaskTitan.Lib.Tests;

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

public class FakeTtask
{
    public FakeTtask() { }

    internal static IEnumerable<TTask> Generate(int quantity)
    {
        Randomizer.Seed = new Random(123456);
        var faker = new Faker<TTask>()
        .CustomInstantiator(f => TTask.CreateNew(f.Lorem.Sentence(3, 2)))
        .RuleFor(t => t.CreatedAt, f => f.Date.Recent(30))
        .RuleFor(t => t.DueDate, (f, t) => f.Date.FutureDateOnly(1));

        return faker.Generate(quantity);
    }
}
