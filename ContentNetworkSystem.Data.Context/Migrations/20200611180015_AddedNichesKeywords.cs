using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContentNetworkSystem.Migrations
{
    public partial class AddedNichesKeywords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NicheId",
                table: "Project",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Niche",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Niche", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Keyword",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NicheId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keyword", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Keyword_Niche_NicheId",
                        column: x => x.NicheId,
                        principalTable: "Niche",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Project_NicheId",
                table: "Project",
                column: "NicheId");

            migrationBuilder.CreateIndex(
                name: "IX_Keyword_NicheId_Name",
                table: "Keyword",
                columns: new[] { "NicheId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Niche_Name",
                table: "Niche",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Niche_NicheId",
                table: "Project",
                column: "NicheId",
                principalTable: "Niche",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_Niche_NicheId",
                table: "Project");

            migrationBuilder.DropTable(
                name: "Keyword");

            migrationBuilder.DropTable(
                name: "Niche");

            migrationBuilder.DropIndex(
                name: "IX_Project_NicheId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "NicheId",
                table: "Project");
        }
    }
}
