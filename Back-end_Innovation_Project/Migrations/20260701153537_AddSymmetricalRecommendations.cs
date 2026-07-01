using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Back_end_Innovation_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddSymmetricalRecommendations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecommendedWaterLiters",
                table: "EvaluationHistory",
                newName: "RecommendedEnvWaterLiters");

            migrationBuilder.RenameColumn(
                name: "RecommendedModel",
                table: "EvaluationHistory",
                newName: "RecommendedEnvModel");

            migrationBuilder.RenameColumn(
                name: "RecommendedEnergyKwh",
                table: "EvaluationHistory",
                newName: "RecommendedEnvEnergyKwh");

            migrationBuilder.RenameColumn(
                name: "RecommendedComplexity",
                table: "EvaluationHistory",
                newName: "RecommendedEnvComplexity");

            migrationBuilder.AddColumn<string>(
                name: "RecommendedEcoComplexity",
                table: "EvaluationHistory",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "RecommendedEcoCostUsd",
                table: "EvaluationHistory",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RecommendedEcoEnergyKwh",
                table: "EvaluationHistory",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "RecommendedEcoModel",
                table: "EvaluationHistory",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "RecommendedEcoWaterLiters",
                table: "EvaluationHistory",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RecommendedEnvCostUsd",
                table: "EvaluationHistory",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecommendedEcoComplexity",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "RecommendedEcoCostUsd",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "RecommendedEcoEnergyKwh",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "RecommendedEcoModel",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "RecommendedEcoWaterLiters",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "RecommendedEnvCostUsd",
                table: "EvaluationHistory");

            migrationBuilder.RenameColumn(
                name: "RecommendedEnvWaterLiters",
                table: "EvaluationHistory",
                newName: "RecommendedWaterLiters");

            migrationBuilder.RenameColumn(
                name: "RecommendedEnvModel",
                table: "EvaluationHistory",
                newName: "RecommendedModel");

            migrationBuilder.RenameColumn(
                name: "RecommendedEnvEnergyKwh",
                table: "EvaluationHistory",
                newName: "RecommendedEnergyKwh");

            migrationBuilder.RenameColumn(
                name: "RecommendedEnvComplexity",
                table: "EvaluationHistory",
                newName: "RecommendedComplexity");
        }
    }
}
