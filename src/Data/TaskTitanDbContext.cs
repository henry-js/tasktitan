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

        // modelBuilder.Entity<MyTask>().HasData(
        //     [
        //         MyTask.CreateNew("Basic task"),
        //         MyTask.CreateNew("Wash the dog"),
        //         MyTask.CreateNew("Feed the cats"),
        //     ]
        // );
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Database.EnsureCreated();
    }
}
