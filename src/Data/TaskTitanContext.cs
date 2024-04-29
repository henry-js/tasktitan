using Microsoft.EntityFrameworkCore;

using TaskTitan.Domain;

using Task = TaskTitan.Domain.Task;

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
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}
