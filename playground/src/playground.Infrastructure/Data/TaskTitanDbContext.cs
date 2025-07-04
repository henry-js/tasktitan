using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.ValueGeneration;

using playground.Core;

using TaskTitan.Infrastructure.Data;

namespace playground.Infrastructure.Data;

public class TaskTitanDbContext : DbContext
{
    public DbSet<TaskItem> TaskItems { get; set; } = null!;

    private readonly string _dbPath;

    public TaskTitanDbContext(string dbPath = "tasktitan.db")
    {
        _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbPath);
        var dir = Path.GetDirectoryName(_dbPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    public TaskTitanDbContext(DbContextOptions<TaskTitanDbContext> options) : base(options)
    {
        _dbPath = "tasktitan_di.db";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w => w.Ignore([RelationalEventId.PendingModelChangesWarning]));
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TaskItem>(b
            => b.Property(t
                => t.Uuid).HasValueGenerator<TaskUuidValueGenerator>());

        FakeData.Init(1000);

        builder.Entity<TaskItem>().HasData(FakeData.TaskItems);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.RegisterAllInVogenEfCoreConverters();
    }

    internal class TaskUuidValueGenerator : ValueGenerator<TaskUuid>
    {
        public override TaskUuid Next(EntityEntry entry)
            => TaskUuid.From(Guid.CreateVersion7());

        public override bool GeneratesTemporaryValues => false;
    }
}

public interface ITaskItemRepository
{
    Task<TaskItem?> GetByIdAsync(TaskId id);
    Task<TaskItem?> GetByUuidAsync(TaskUuid uuid);
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<IEnumerable<TaskItem>> FindAsync(Expression<Func<TaskItem, bool>> predicate);
    Task AddAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskId id);
    Task SaveChangesAsync();
}

public class SqliteTaskItemRepository : ITaskItemRepository
{
    private readonly TaskTitanDbContext _context;

    public SqliteTaskItemRepository(TaskTitanDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAsync(TaskItem task)
    {
        ArgumentNullException.ThrowIfNull(task);
        await _context.TaskItems.AddAsync(task);
    }

    public async Task DeleteAsync(TaskId id)
    {
        var task = await _context.TaskItems.FindAsync(id);
        if (task != null)
        {
            _context.TaskItems.Remove(task);
        }
    }

    public async Task<IEnumerable<TaskItem>> FindAsync(Expression<Func<TaskItem, bool>> predicate)
    {
        return await _context.TaskItems.Where(predicate).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        return await _context.TaskItems.AsNoTracking().ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(TaskId id)
    {
        return await _context.TaskItems.FindAsync(id);
    }

    public async Task<TaskItem?> GetByUuidAsync(TaskUuid uuid)
    {
        // AsNoTracking suitable if just reading
        return await _context.TaskItems
                             .AsNoTracking()
                             .FirstOrDefaultAsync(t => t.Uuid == uuid);
    }

    public async Task UpdateAsync(TaskItem task)
    {
        ArgumentNullException.ThrowIfNull(task);

        // EF Core tracks changes to attached entities.
        // If the task instance passed in might not be tracked, you need to handle it.

        // Option 1: Find existing, then update (safe, ensures entity exists)
        var existingTask = await _context.TaskItems.FindAsync(task.Id);
        if (existingTask != null)
        {
            // Copy values from the passed-in task to the tracked one
            _context.Entry(existingTask).CurrentValues.SetValues(task);
            // EF Core automatically marks state as Modified
        }
        else
        {
            throw new InvalidOperationException($"Task with ID {task.Id} not found for update.");
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}