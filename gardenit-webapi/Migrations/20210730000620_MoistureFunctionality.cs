using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace gardenit_webapi.Migrations
{
    public partial class MoistureFunctionality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Seconds",
                table: "Watering",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasDevice",
                table: "Plants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PollPeriodSeconds",
                table: "Plants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MoistureReading",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Plant = table.Column<Guid>(type: "uuid", nullable: false),
                    ReadDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoistureReading", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoistureReading_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoistureReading_PlantId",
                table: "MoistureReading",
                column: "PlantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoistureReading");

            migrationBuilder.DropColumn(
                name: "Seconds",
                table: "Watering");

            migrationBuilder.DropColumn(
                name: "HasDevice",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "PollPeriodSeconds",
                table: "Plants");
        }
    }
}
