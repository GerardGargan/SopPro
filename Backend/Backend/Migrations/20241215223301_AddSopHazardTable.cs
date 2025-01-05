using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddSopHazardTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SopHazards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SopVersionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControlMeasure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RiskLevel = table.Column<int>(type: "int", nullable: true),
                    OrganisationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SopHazards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SopHazards_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SopHazards_SopVersions_SopVersionId",
                        column: x => x.SopVersionId,
                        principalTable: "SopVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SopHazards_OrganisationId",
                table: "SopHazards",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_SopHazards_SopVersionId",
                table: "SopHazards",
                column: "SopVersionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SopHazards");
        }
    }
}
