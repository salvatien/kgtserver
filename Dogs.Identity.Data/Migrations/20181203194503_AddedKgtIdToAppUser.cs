using Microsoft.EntityFrameworkCore.Migrations;

namespace Dogs.Identity.Data.Migrations
{
    public partial class AddedKgtIdToAppUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KgtId",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KgtId",
                table: "AspNetUsers");
        }
    }
}
