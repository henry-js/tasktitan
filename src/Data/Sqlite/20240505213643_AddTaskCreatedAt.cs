using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Sqlite
{
    /// <inheritdoc />
    public partial class AddTaskCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: "IX6EVYKu");

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: "VGC8kG3z");

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: "Xtf2Kvhr");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "tasks",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "tasks",
                columns: new[] { "Id", "CreatedAt", "Description" },
                values: new object[,]
                {
                    { "BqlWvjNY", new DateTime(2024, 5, 5, 21, 36, 42, 619, DateTimeKind.Utc).AddTicks(5544), "Basic task" },
                    { "qGjh4lHm", new DateTime(2024, 5, 5, 21, 36, 42, 619, DateTimeKind.Utc).AddTicks(5555), "Feed the cats" },
                    { "sCBNmoJr", new DateTime(2024, 5, 5, 21, 36, 42, 619, DateTimeKind.Utc).AddTicks(5550), "Wash the dog" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: "BqlWvjNY");

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: "qGjh4lHm");

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: "sCBNmoJr");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "tasks");

            migrationBuilder.InsertData(
                table: "tasks",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { "IX6EVYKu", "Feed the cats" },
                    { "VGC8kG3z", "Wash the dog" },
                    { "Xtf2Kvhr", "Basic task" }
                });
        }
    }
}
