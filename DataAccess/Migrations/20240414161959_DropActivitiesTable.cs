using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CFPService.Migrations
{
    /// <inheritdoc />
    public partial class DropActivitiesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Activities_ActivityName",
                table: "Applications");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Applications_ActivityName",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ActivityName",
                table: "Applications");

            migrationBuilder.AddColumn<int>(
                name: "Activity",
                table: "Applications",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activity",
                table: "Applications");

            migrationBuilder.AddColumn<string>(
                name: "ActivityName",
                table: "Applications",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Activity = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Activity);
                });

            migrationBuilder.InsertData(
                table: "Activities",
                columns: new[] { "Activity", "Description" },
                values: new object[,]
                {
                    { "Discussion", "Дискуссия / круглый стол, 40-50 минут" },
                    { "Masterclass", "Мастеркласс, 1-2 часа" },
                    { "Report", "Доклад, 35-45 минут" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ActivityName",
                table: "Applications",
                column: "ActivityName");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Activities_ActivityName",
                table: "Applications",
                column: "ActivityName",
                principalTable: "Activities",
                principalColumn: "Activity");
        }
    }
}
