using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentNetworkSystem.Migrations
{
    public partial class WordpressModelFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "XmlRPCUrl",
                table: "Content");

            migrationBuilder.AddColumn<int>(
                name: "BlogId",
                table: "Content",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Content",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Content",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlogId",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Content");

            migrationBuilder.AddColumn<string>(
                name: "XmlRPCUrl",
                table: "Content",
                type: "text",
                nullable: true);
        }
    }
}
