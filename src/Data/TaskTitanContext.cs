using Microsoft.EntityFrameworkCore;

using TaskTitan.Core;

using MyTask = TaskTitan.Core.MyTask;

namespace TaskTitan.Data;

public class TaskTitanDbContext : DbContext
{
    public TaskTitanDbContext(DbContextOptions<TaskTitanDbContext> options)
    : base(options)
    {
    }

    public DbSet<MyTask> Tasks => base.Set<MyTask>();

    public void Commit() => this.SaveChanges();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MyTask>()
        .Property(task => task.Id)
        .HasConversion(id => id.Value.ToString(), value => new MyTaskId(value));

        modelBuilder.Entity<MyTask>().ToTable("tasks");
        modelBuilder.Entity<MyTask>().HasKey(t => t.Id);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MyTask>().HasData(
            [
                MyTask.CreateNew("Basic task"),
                MyTask.CreateNew("Wash the dog"),
                MyTask.CreateNew("Feed the cats"),
            ]
        );
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Database.EnsureCreated();
    }
}
