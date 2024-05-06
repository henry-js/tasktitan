using Microsoft.EntityFrameworkCore.Migrations;

using TaskTitan.Core;

#nullable disable

namespace Data.Sqlite
{
    /// <inheritdoc />
    public partial class PendingTasksView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"""
CREATE VIEW {PendingTTask.ViewName} as
SELECT
    *,
    row_number() OVER ( ORDER BY CreatedAt) RowId
FROM tasks
WHERE
    State = "Pending"
""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"""
drop view {PendingTTask.ViewName};
""");
        }
    }
}
