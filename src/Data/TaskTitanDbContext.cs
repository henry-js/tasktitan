using TTask = TaskTitan.Core.TTask;

namespace TaskTitan.Data;

public class TaskTitanDbContext : DbContext
{
    // public const string MigrationExe = "tasksdb_migrations.exe";
    public TaskTitanDbContext(DbContextOptions<TaskTitanDbContext> options)
    : base(options)
    {
    }

    public DbSet<TTask> Tasks => base.Set<TTask>();

    public void Commit() => this.SaveChanges();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TTask>()
            .ToTable("tasks")
            .HasKey(t => t.Id);
        modelBuilder.Entity<TTask>()
            .Property(task => task.Id)
            .HasConversion(id => id.Value.ToString(), value => new TTaskId(value));
        modelBuilder.Entity<TTask>()
            .HasQueryFilter(t => t.State == TTaskState.Pending);
        modelBuilder.Entity<TTask>()
            .Property(t => t.State)
            .HasConversion<string>();

        modelBuilder.Entity<TTask>().OwnsOne(
            task => task.Metadata, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder
                    .ToJson();
            }
        );

        // modelBuilder.Entity<PendingTTask>()
        //     .ToView(PendingTTask.ViewName);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Database.EnsureCreated();
    }
}
