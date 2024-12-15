using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPpeAndSopStepPpeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ppe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ppe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SopStepPpe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SopStepId = table.Column<int>(type: "int", nullable: false),
                    PpeId = table.Column<int>(type: "int", nullable: false),
                    OrganisationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SopStepPpe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SopStepPpe_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SopStepPpe_Ppe_PpeId",
                        column: x => x.PpeId,
                        principalTable: "Ppe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SopStepPpe_SopSteps_SopStepId",
                        column: x => x.SopStepId,
                        principalTable: "SopSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SopStepPpe_OrganisationId",
                table: "SopStepPpe",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_SopStepPpe_PpeId",
                table: "SopStepPpe",
                column: "PpeId");

            migrationBuilder.CreateIndex(
                name: "IX_SopStepPpe_SopStepId",
                table: "SopStepPpe",
                column: "SopStepId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SopStepPpe");

            migrationBuilder.DropTable(
                name: "Ppe");
        }
    }
}
