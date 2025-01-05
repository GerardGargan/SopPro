using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class SopAndSopVersionsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Organisations_OrganisationId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Sops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isAiGenerated = table.Column<bool>(type: "bit", nullable: true),
                    OrganisationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sops_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Sops_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SopVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SopId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApprovedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganisationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SopVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SopVersions_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SopVersions_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SopVersions_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SopVersions_Sops_SopId",
                        column: x => x.SopId,
                        principalTable: "Sops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sops_DepartmentId",
                table: "Sops",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Sops_OrganisationId",
                table: "Sops",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_SopVersions_ApprovedById",
                table: "SopVersions",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_SopVersions_AuthorId",
                table: "SopVersions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_SopVersions_OrganisationId",
                table: "SopVersions",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_SopVersions_SopId",
                table: "SopVersions",
                column: "SopId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Organisations_OrganisationId",
                table: "AspNetUsers",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Organisations_OrganisationId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "SopVersions");

            migrationBuilder.DropTable(
                name: "Sops");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Organisations_OrganisationId",
                table: "AspNetUsers",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
