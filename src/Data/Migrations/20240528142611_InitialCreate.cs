using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    State = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "Pending"),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Modified = table.Column<DateTime>(type: "TEXT", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Due = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Until = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Wait = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Started = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ended = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Scheduled = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks", x => x.Id);
                });
            migrationBuilder.Sql(
"""
CREATE TRIGGER IF NOT EXISTS task_MODIFIED
	AFTER UPDATE
	ON tasks
    FOR EACH ROW
BEGIN
	UPDATE tasks
		SET Modified = CURRENT_TIMESTAMP
	WHERE Id = old.Id;
END;
""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tasks");
        }
    }
}
