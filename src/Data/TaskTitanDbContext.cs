using static TaskTitan.Data.DbConstants;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TaskTitan.Data;

public class TaskTitanDbContext : DbContext
{
    public TaskTitanDbContext(DbContextOptions<TaskTitanDbContext> options)
    : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => base.Set<TaskItem>();

    public void Commit() => this.SaveChanges();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>()
            .ToTable(TasksTable)
            .HasKey(t => t.Id);
        modelBuilder.Entity<TaskItem>()
            .Property(task => task.Id)
            .HasConversion(id => id.Value.ToString(), value => new TaskItemId(value));
        modelBuilder.Entity<TaskItem>()
            .HasQueryFilter(t => t.State == TaskItemState.Pending);
        modelBuilder.Entity<TaskItem>()
            .Property(t => t.State)
            .HasConversion<string>()
            .HasDefaultValue(TaskItemState.Pending);
        modelBuilder.Entity<TaskItem>()
            .Property(task => task.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        modelBuilder.Entity<TaskItem>()
            .Property(task => task.Modified)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        modelBuilder.Entity<TaskItem>().OwnsOne(
            task => task.Metadata, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder
                    .ToJson();
            }
        );

    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        // Database.EnsureCreated();
    }
}
