using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Sqlite
{
    /// <inheritdoc />
    public partial class RenameTaskFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "tasks",
                newName: "Wait");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "tasks",
                newName: "Modified");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "tasks",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Due",
                table: "tasks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Ended",
                table: "tasks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Scheduled",
                table: "tasks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Started",
                table: "tasks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Until",
                table: "tasks",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "Due",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "Ended",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "Scheduled",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "Started",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "Until",
                table: "tasks");

            migrationBuilder.RenameColumn(
                name: "Wait",
                table: "tasks",
                newName: "DueDate");

            migrationBuilder.RenameColumn(
                name: "Modified",
                table: "tasks",
                newName: "CreatedAt");
        }
    }
}
