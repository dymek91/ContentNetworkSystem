using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentNetworkSystem.Migrations
{
    public partial class AddedNicheTGFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TextGenerationCategoryId",
                table: "Niche",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TextGenerationLowQCategoryId",
                table: "Niche",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextGenerationCategoryId",
                table: "Niche");

            migrationBuilder.DropColumn(
                name: "TextGenerationLowQCategoryId",
                table: "Niche");
        }
    }
}
