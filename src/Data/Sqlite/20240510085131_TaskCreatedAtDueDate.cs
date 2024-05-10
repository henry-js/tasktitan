using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Sqlite
{
    /// <inheritdoc />
    public partial class TaskCreatedAtDueDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "tasks",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "tasks");
        }
    }
}
