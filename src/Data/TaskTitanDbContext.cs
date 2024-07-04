using static TaskTitan.Data.DbConstants;
using Microsoft.EntityFrameworkCore.Metadata;
using TaskTitan.Core.Enums;

namespace TaskTitan.Data;

public class TaskTitanDbContext : DbContext
{
    public TaskTitanDbContext(DbContextOptions<TaskTitanDbContext> options)
    : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => base.Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>()
            .ToTable(TasksTable.Name)
            .HasKey(t => t.Id);
        modelBuilder.Entity<TaskItem>().Property(t => t.Id).HasColumnName(TaskItemAttribute.Id);
        modelBuilder.Entity<TaskItem>().Property(t => t.Description).HasColumnName(TaskItemAttribute.Description);
        modelBuilder.Entity<TaskItem>().Property(t => t.Status).HasColumnName(TaskItemAttribute.Status);
        modelBuilder.Entity<TaskItem>().Property(t => t.Project).HasColumnName(TaskItemAttribute.Project);
        modelBuilder.Entity<TaskItem>().Property(t => t.Due).HasColumnName(TaskItemAttribute.Due);
        // modelBuilder.Entity<TaskItem>().Property(t => t.Recur).HasColumnName(TaskItemAttribute.Recur);
        modelBuilder.Entity<TaskItem>().Property(t => t.Until).HasColumnName(TaskItemAttribute.Until);
        modelBuilder.Entity<TaskItem>().Property(t => t.Wait).HasColumnName(TaskItemAttribute.Wait);
        modelBuilder.Entity<TaskItem>().Property(t => t.Entry).HasColumnName(TaskItemAttribute.Entry);
        modelBuilder.Entity<TaskItem>().Property(t => t.End).HasColumnName(TaskItemAttribute.End);
        modelBuilder.Entity<TaskItem>().Property(t => t.Start).HasColumnName(TaskItemAttribute.Start);
        modelBuilder.Entity<TaskItem>().Property(t => t.Modified).HasColumnName(TaskItemAttribute.Modified);
        // modelBuilder.Entity<TaskItem>().Property(t => t.Depends).HasColumnName(TaskItemAttribute.Depends);
        modelBuilder.Entity<TaskItem>().Property(t => t.Id).HasColumnName(TaskItemAttribute.Id);
        modelBuilder.Entity<TaskItem>()
            .Property(task => task.Id)
            .HasConversion(id => id.Value.ToString(), value => new TaskItemId(value));
        modelBuilder.Entity<TaskItem>()
            .HasQueryFilter(t => t.Status == TaskItemState.Pending);
        modelBuilder.Entity<TaskItem>()
            .Property(t => t.Status)
            .HasConversion(
                v => v.ToString(),
                v => (TaskItemState)v
            )
            .HasDefaultValue(TaskItemState.Pending);
        modelBuilder.Entity<TaskItem>()
            .Property(task => task.Entry)
            .HasDefaultValueSql("strftime('%Y-%m-%dT%H:%M:%fZ', 'now')")
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        modelBuilder.Entity<TaskItem>()
            .Property(task => task.Modified)
            .HasDefaultValueSql("strftime('%Y-%m-%dT%H:%M:%fZ', 'now')")
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        // modelBuilder.Entity<TaskItem>().OwnsOne(
        //     task => task.Metadata, ownedNavigationBuilder =>
        //     {
        //         ownedNavigationBuilder
        //             .ToJson();
        //     }
        // );
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        // Database.EnsureCreated();
    }
}
