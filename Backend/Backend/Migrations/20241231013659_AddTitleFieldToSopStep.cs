using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleFieldToSopStep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "reference",
                table: "Sops",
                newName: "Reference");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "SopSteps",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "SopSteps");

            migrationBuilder.RenameColumn(
                name: "Reference",
                table: "Sops",
                newName: "reference");
        }
    }
}
