using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class TaskTriggersAndViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(DbConstants.TasksTable.CreateViewTasksWithRowId);
            migrationBuilder.Sql(DbConstants.TasksTable.CreateTaskModifiedTrigger);
            migrationBuilder.Sql(DbConstants.TasksTable.CreateTaskCreatedTrigger);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(DbConstants.TasksTable.DropViewTasksWithRowId);
            migrationBuilder.Sql(DbConstants.TasksTable.DropTaskModifiedTrigger);
            migrationBuilder.Sql(DbConstants.TasksTable.DropTaskCreatedTrigger);
        }
    }
}
