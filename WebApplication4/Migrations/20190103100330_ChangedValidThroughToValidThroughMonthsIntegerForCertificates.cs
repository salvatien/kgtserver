using Microsoft.EntityFrameworkCore.Migrations;

namespace DogsServer.Migrations
{
    public partial class ChangedValidThroughToValidThroughMonthsIntegerForCertificates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidThroughTicks",
                table: "Certificates");

            migrationBuilder.AddColumn<int>(
                name: "ValidThroughMonths",
                table: "Certificates",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidThroughMonths",
                table: "Certificates");

            migrationBuilder.AddColumn<long>(
                name: "ValidThroughTicks",
                table: "Certificates",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
