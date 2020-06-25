using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentNetworkSystem.Migrations
{
    public partial class AddedFieldsToWordpress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AddThumbnail",
                table: "Content",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorityLinksCount",
                table: "Content",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagesCount",
                table: "Content",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideosCount",
                table: "Content",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddThumbnail",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "AuthorityLinksCount",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "ImagesCount",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "VideosCount",
                table: "Content");
        }
    }
}
