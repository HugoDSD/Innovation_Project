using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Back_end_Innovation_Project.Migrations
{
    /// <inheritdoc />
    public partial class RefonteSobriaModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoursSaved",
                table: "EvaluationHistory");

            migrationBuilder.RenameColumn(
                name: "RiskScore",
                table: "EvaluationHistory",
                newName: "ValueSavedEur");

            migrationBuilder.AddColumn<int>(
                name: "EconomicRating",
                table: "EvaluationHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EfficiencyRating",
                table: "EvaluationHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EnvironmentalRating",
                table: "EvaluationHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RiskRating",
                table: "EvaluationHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VerdictLevel",
                table: "EvaluationHistory",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EconomicRating",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "EfficiencyRating",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "EnvironmentalRating",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "RiskRating",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "VerdictLevel",
                table: "EvaluationHistory");

            migrationBuilder.RenameColumn(
                name: "ValueSavedEur",
                table: "EvaluationHistory",
                newName: "RiskScore");

            migrationBuilder.AddColumn<double>(
                name: "HoursSaved",
                table: "EvaluationHistory",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
