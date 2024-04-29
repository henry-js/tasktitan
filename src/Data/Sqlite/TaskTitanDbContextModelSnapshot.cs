﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskTitan.Data;

#nullable disable

namespace Data.Sqlite
{
    [DbContext(typeof(TaskTitanDbContext))]
    partial class TaskTitanDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Tasks")
                .HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("TaskTitan.Domain.Task", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("tasks", "Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
