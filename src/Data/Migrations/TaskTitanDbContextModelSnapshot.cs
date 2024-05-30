﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskTitan.Data;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(TaskTitanDbContext))]
    partial class TaskTitanDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("TaskTitan.Core.TaskItem", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Due")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Ended")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Modified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<DateTime?>("Scheduled")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Started")
                        .HasColumnType("TEXT");

                    b.Property<string>("State")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("Pending");

                    b.Property<DateTime?>("Until")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Wait")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("tasks", (string)null);
                });

            modelBuilder.Entity("TaskTitan.Data.TaskItemWithRowId", b =>
                {
                    b.HasBaseType("TaskTitan.Core.TaskItem");

                    b.ToTable((string)null);

                    b.ToView("pending_tasks", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}