using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Sqlite
{
    /// <inheritdoc />
    public partial class ChangeDueDateType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowId",
                table: "tasks");

            migrationBuilder.AlterColumn<string>(
                name: "Metadata",
                table: "tasks",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Metadata",
                table: "tasks",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "RowId",
                table: "tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
