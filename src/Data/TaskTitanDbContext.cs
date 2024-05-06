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
    public DbSet<PendingTTask> PendingTasks => base.Set<PendingTTask>();

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
            .Property(t => t.State)
            .HasConversion<string>();

        // modelBuilder.Entity<TTask>().HasData(
        //     [
        //         TTask.CreateNew("Basic task"),
        //         TTask.CreateNew("Wash the dog"),
        //         TTask.CreateNew("Feed the cats"),
        //         TTask.CreateNew("Started task").Start(),
        //         TTask.CreateNew("Completed task").Complete(),
        //     ]
        // );

        modelBuilder.Entity<PendingTTask>()
            .ToView(PendingTTask.ViewName);
        // .HasKey(pt => pt.RowId);

    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Database.EnsureCreated();
    }
}
