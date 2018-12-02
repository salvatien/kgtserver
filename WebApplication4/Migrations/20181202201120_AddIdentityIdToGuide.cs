using Microsoft.EntityFrameworkCore.Migrations;

namespace DogsServer.Migrations
{
    public partial class AddIdentityIdToGuide : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityId",
                table: "Guides",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityId",
                table: "Guides");
        }
    }
}
