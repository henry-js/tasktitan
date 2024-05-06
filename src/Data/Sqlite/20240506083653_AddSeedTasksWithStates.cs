using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Sqlite
{
    /// <inheritdoc />
    public partial class AddSeedTasksWithStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "tasks",
                columns: new[] { "Id", "CreatedAt", "Description", "State" },
                values: new object[,]
                {
                    { "aI50VPJR", new DateTime(2024, 5, 6, 8, 36, 53, 13, DateTimeKind.Utc).AddTicks(3690), "Completed task", "Done" },
                    { "L2GxtC0R", new DateTime(2024, 5, 6, 8, 36, 53, 13, DateTimeKind.Utc).AddTicks(3676), "Started task", "Started" },
                    { "TbNdVCnL", new DateTime(2024, 5, 6, 8, 36, 53, 13, DateTimeKind.Utc).AddTicks(3721), "Basic task", "Pending" },
                    { "WDvXCLgN", new DateTime(2024, 5, 6, 8, 36, 53, 13, DateTimeKind.Utc).AddTicks(3737), "Feed the cats", "Pending" },
                    { "YHwj0rTr", new DateTime(2024, 5, 6, 8, 36, 53, 13, DateTimeKind.Utc).AddTicks(3729), "Wash the dog", "Pending" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: "aI50VPJR");

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: "L2GxtC0R");

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: "TbNdVCnL");

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: "WDvXCLgN");

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: "YHwj0rTr");
        }
    }
}
