using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Back_end_Innovation_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddSymmetricalAndQualityRecommendations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecommendedQualityComplexity",
                table: "EvaluationHistory",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "RecommendedQualityCostUsd",
                table: "EvaluationHistory",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RecommendedQualityEnergyKwh",
                table: "EvaluationHistory",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "RecommendedQualityModel",
                table: "EvaluationHistory",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "RecommendedQualityWaterLiters",
                table: "EvaluationHistory",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecommendedQualityComplexity",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "RecommendedQualityCostUsd",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "RecommendedQualityEnergyKwh",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "RecommendedQualityModel",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "RecommendedQualityWaterLiters",
                table: "EvaluationHistory");
        }
    }
}
