using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Back_end_Innovation_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddRecommendations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecommendedComplexity",
                table: "EvaluationHistory",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "RecommendedEnergyKwh",
                table: "EvaluationHistory",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "RecommendedModel",
                table: "EvaluationHistory",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "RecommendedWaterLiters",
                table: "EvaluationHistory",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecommendedComplexity",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "RecommendedEnergyKwh",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "RecommendedModel",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "RecommendedWaterLiters",
                table: "EvaluationHistory");
        }
    }
}
