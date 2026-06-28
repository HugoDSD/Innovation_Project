using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnovationProject.Migrations
{
    /// <inheritdoc />
    public partial class AddFullEvaluationMetrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "EstimatedCarbonFootprint",
                table: "EvaluationHistory");

            migrationBuilder.RenameColumn(
                name: "ProjectName",
                table: "EvaluationHistory",
                newName: "ModelName");

            migrationBuilder.AlterColumn<string>(
                name: "AiScore",
                table: "EvaluationHistory",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CarbonFootprint",
                table: "EvaluationHistory",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CostUsd",
                table: "EvaluationHistory",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EnergyKwh",
                table: "EvaluationHistory",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "HoursSaved",
                table: "EvaluationHistory",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "EvaluationHistory",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "RiskScore",
                table: "EvaluationHistory",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WaterFootprintLiters",
                table: "EvaluationHistory",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarbonFootprint",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "CostUsd",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "EnergyKwh",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "HoursSaved",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "RiskScore",
                table: "EvaluationHistory");

            migrationBuilder.DropColumn(
                name: "WaterFootprintLiters",
                table: "EvaluationHistory");

            migrationBuilder.RenameColumn(
                name: "ModelName",
                table: "EvaluationHistory",
                newName: "ProjectName");

            migrationBuilder.AlterColumn<string>(
                name: "AiScore",
                table: "EvaluationHistory",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "EvaluationHistory",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "EstimatedCarbonFootprint",
                table: "EvaluationHistory",
                type: "double precision",
                nullable: true);
        }
    }
}
