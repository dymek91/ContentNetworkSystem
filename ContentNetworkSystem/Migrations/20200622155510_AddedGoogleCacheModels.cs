using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContentNetworkSystem.Migrations
{
    public partial class AddedGoogleCacheModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImagesResult",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    NicheId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagesResult", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ImagesResult_Niche_NicheId",
                        column: x => x.NicheId,
                        principalTable: "Niche",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YoutubeResult",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VideoId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    NicheId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YoutubeResult", x => x.ID);
                    table.ForeignKey(
                        name: "FK_YoutubeResult_Niche_NicheId",
                        column: x => x.NicheId,
                        principalTable: "Niche",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImagesResult_NicheId",
                table: "ImagesResult",
                column: "NicheId");

            migrationBuilder.CreateIndex(
                name: "IX_YoutubeResult_NicheId",
                table: "YoutubeResult",
                column: "NicheId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImagesResult");

            migrationBuilder.DropTable(
                name: "YoutubeResult");
        }
    }
}
