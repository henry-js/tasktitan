using Microsoft.EntityFrameworkCore;

using TaskTitan.Core;

using Task = TaskTitan.Core.Task;

namespace TaskTitan.Data;

public class TaskTitanDbContext : DbContext
{
    public TaskTitanDbContext(DbContextOptions<TaskTitanDbContext> options)
    : base(options)
    {
    }

    public DbSet<Task> Tasks => base.Set<Task>();

    public void Commit() => this.SaveChanges();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>()
        .Property(task => task.Id)
        .HasConversion(id => id.Value.ToString(), value => new TaskId(value));

        modelBuilder.Entity<Task>().ToTable("tasks");
        modelBuilder.Entity<Task>().HasKey(t => t.Id);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Task>().HasData(
            [
                Task.CreateNew("Basic task"),
                Task.CreateNew("Wash the dog"),
                Task.CreateNew("Feed the cats"),
            ]
        );
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Database.EnsureCreated();
    }
}
