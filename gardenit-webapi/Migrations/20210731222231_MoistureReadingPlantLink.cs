using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace gardenit_webapi.Migrations
{
    public partial class MoistureReadingPlantLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Plant",
                table: "MoistureReading");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Plant",
                table: "MoistureReading",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
