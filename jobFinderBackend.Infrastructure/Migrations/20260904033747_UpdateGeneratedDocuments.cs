using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jobFinderBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGeneratedDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileUrl",
                table: "GeneratedCVs",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "FileUrl",
                table: "GeneratedCoverLetters",
                newName: "Content");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "GeneratedCVs",
                newName: "FileUrl");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "GeneratedCoverLetters",
                newName: "FileUrl");
        }
    }
}
