using Microsoft.EntityFrameworkCore;

using TaskTitan.Core;

using TTask = TaskTitan.Core.TTask;

namespace TaskTitan.Data;

public class TaskTitanDbContext : DbContext
{
    public TaskTitanDbContext(DbContextOptions<TaskTitanDbContext> options)
    : base(options)
    {
    }

    public DbSet<TTask> Tasks => base.Set<TTask>();

    public void Commit() => this.SaveChanges();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TTask>()
        .Property(task => task.Id)
        .HasConversion(id => id.Value.ToString(), value => new TTaskId(value));

        modelBuilder.Entity<TTask>().ToTable("tasks");
        modelBuilder.Entity<TTask>().HasKey(t => t.Id);

        modelBuilder.Entity<TTask>()
            .Property(t => t.State)
            .HasConversion<string>();

        base.OnModelCreating(modelBuilder);

        var startedTask = TTask.CreateNew("Started task");
        startedTask.Start();
        var completedTask = TTask.CreateNew("Completed task");
        completedTask.Complete();

        modelBuilder.Entity<TTask>().HasData(
            [
                TTask.CreateNew("Basic task"),
                TTask.CreateNew("Wash the dog"),
                TTask.CreateNew("Feed the cats"),
                startedTask,
                completedTask,
            ]
        );

    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Database.EnsureCreated();
    }
}
