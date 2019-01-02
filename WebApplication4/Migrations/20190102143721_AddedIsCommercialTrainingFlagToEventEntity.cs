using Microsoft.EntityFrameworkCore.Migrations;

namespace DogsServer.Migrations
{
    public partial class AddedIsCommercialTrainingFlagToEventEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCommercialTraining",
                table: "Events",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCommercialTraining",
                table: "Events");
        }
    }
}
