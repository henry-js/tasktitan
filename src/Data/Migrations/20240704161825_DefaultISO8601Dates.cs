using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class DefaultISO8601Dates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "tasks",
                type: "TEXT",
                nullable: false,
                defaultValue: "pending",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldDefaultValue: "Pending");

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified",
                table: "tasks",
                type: "TEXT",
                nullable: true,
                defaultValueSql: "strftime('%Y-%m-%dT%H:%M:%fZ', 'now')",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "entry",
                table: "tasks",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "strftime('%Y-%m-%dT%H:%M:%fZ', 'now')",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "tasks",
                type: "TEXT",
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldDefaultValue: "pending");

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified",
                table: "tasks",
                type: "TEXT",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true,
                oldDefaultValueSql: "strftime('%Y-%m-%dT%H:%M:%fZ', 'now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "entry",
                table: "tasks",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "strftime('%Y-%m-%dT%H:%M:%fZ', 'now')");
        }
    }
}
