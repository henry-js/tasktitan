using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Sqlite
{
    /// <inheritdoc />
    public partial class SeedBasicTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "tasks",
                schema: "Tasks",
                newName: "tasks");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.EnsureSchema(
                name: "Tasks");

            migrationBuilder.RenameTable(
                name: "tasks",
                newName: "tasks",
                newSchema: "Tasks");
        }
    }
}
