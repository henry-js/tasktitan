﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskTitan.Data;

#nullable disable

namespace Data.Sqlite
{
    [DbContext(typeof(TaskTitanDbContext))]
    [Migration("20240510085131_TaskCreatedAtDueDate")]
    partial class TaskCreatedAtDueDate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("TaskTitan.Core.TTask", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("tasks", (string)null);
                });

            modelBuilder.Entity("TaskTitan.Core.PendingTTask", b =>
                {
                    b.HasBaseType("TaskTitan.Core.TTask");

                    b.Property<int>("RowId")
                        .HasColumnType("INTEGER");

                    b.ToTable((string)null);

                    b.ToView("pending_tasks", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
