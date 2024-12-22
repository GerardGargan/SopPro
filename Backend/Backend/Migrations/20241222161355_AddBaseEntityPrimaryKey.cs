using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseEntityPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Organisations_OrganisationId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Organisations_OrganisationId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_SopHazards_SopVersions_SopVersionId",
                table: "SopHazards");

            migrationBuilder.DropForeignKey(
                name: "FK_SopStepPpe_Ppe_PpeId",
                table: "SopStepPpe");

            migrationBuilder.DropForeignKey(
                name: "FK_SopStepPpe_SopSteps_SopStepId",
                table: "SopStepPpe");

            migrationBuilder.DropForeignKey(
                name: "FK_SopSteps_SopVersions_SopVersionId",
                table: "SopSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_SopVersions_Sops_SopId",
                table: "SopVersions");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Organisations_OrganisationId",
                table: "Departments",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Organisations_OrganisationId",
                table: "Invitations",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SopHazards_SopVersions_SopVersionId",
                table: "SopHazards",
                column: "SopVersionId",
                principalTable: "SopVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SopStepPpe_Ppe_PpeId",
                table: "SopStepPpe",
                column: "PpeId",
                principalTable: "Ppe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SopStepPpe_SopSteps_SopStepId",
                table: "SopStepPpe",
                column: "SopStepId",
                principalTable: "SopSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SopSteps_SopVersions_SopVersionId",
                table: "SopSteps",
                column: "SopVersionId",
                principalTable: "SopVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SopVersions_Sops_SopId",
                table: "SopVersions",
                column: "SopId",
                principalTable: "Sops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Organisations_OrganisationId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Organisations_OrganisationId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_SopHazards_SopVersions_SopVersionId",
                table: "SopHazards");

            migrationBuilder.DropForeignKey(
                name: "FK_SopStepPpe_Ppe_PpeId",
                table: "SopStepPpe");

            migrationBuilder.DropForeignKey(
                name: "FK_SopStepPpe_SopSteps_SopStepId",
                table: "SopStepPpe");

            migrationBuilder.DropForeignKey(
                name: "FK_SopSteps_SopVersions_SopVersionId",
                table: "SopSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_SopVersions_Sops_SopId",
                table: "SopVersions");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Organisations_OrganisationId",
                table: "Departments",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Organisations_OrganisationId",
                table: "Invitations",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SopHazards_SopVersions_SopVersionId",
                table: "SopHazards",
                column: "SopVersionId",
                principalTable: "SopVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SopStepPpe_Ppe_PpeId",
                table: "SopStepPpe",
                column: "PpeId",
                principalTable: "Ppe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SopStepPpe_SopSteps_SopStepId",
                table: "SopStepPpe",
                column: "SopStepId",
                principalTable: "SopSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SopSteps_SopVersions_SopVersionId",
                table: "SopSteps",
                column: "SopVersionId",
                principalTable: "SopVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SopVersions_Sops_SopId",
                table: "SopVersions",
                column: "SopId",
                principalTable: "Sops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
