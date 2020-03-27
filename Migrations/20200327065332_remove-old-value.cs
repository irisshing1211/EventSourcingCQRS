using Microsoft.EntityFrameworkCore.Migrations;

namespace EventSourcingCQRS.Migrations
{
    public partial class removeoldvalue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OldValue",
                table: "EventLogs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OldValue",
                table: "EventLogs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
