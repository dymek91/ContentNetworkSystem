using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentNetworkSystem.Migrations
{
    public partial class AddedWasSuccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WasSuccess",
                table: "Project",
                nullable: true,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WasSuccess",
                table: "Project");
        }
    }
}
