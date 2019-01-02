using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DogsServer.Migrations
{
    public partial class HackForTimeSpanLongerThan24Hours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidThrough",
                table: "Certificates");

            migrationBuilder.AddColumn<long>(
                name: "ValidThroughTicks",
                table: "Certificates",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidThroughTicks",
                table: "Certificates");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ValidThrough",
                table: "Certificates",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
