using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepoInsight.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LetterFrequencies",
                columns: table => new
                {
                    Letter = table.Column<string>(type: "text", nullable: false),
                    RepositoryUrl = table.Column<string>(type: "text", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LetterFrequencies", x => new { x.Letter, x.RepositoryUrl });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LetterFrequencies");
        }
    }
}
