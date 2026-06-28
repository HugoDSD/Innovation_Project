using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnovationProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "EvaluationHistory",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "EvaluationHistory");
        }
    }
}
