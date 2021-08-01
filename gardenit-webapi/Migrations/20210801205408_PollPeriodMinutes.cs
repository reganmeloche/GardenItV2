using Microsoft.EntityFrameworkCore.Migrations;

namespace gardenit_webapi.Migrations
{
    public partial class PollPeriodMinutes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PollPeriodSeconds",
                table: "Plants",
                newName: "PollPeriodMinutes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PollPeriodMinutes",
                table: "Plants",
                newName: "PollPeriodSeconds");
        }
    }
}
