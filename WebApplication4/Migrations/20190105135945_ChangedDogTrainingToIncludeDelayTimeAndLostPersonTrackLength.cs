using Microsoft.EntityFrameworkCore.Migrations;

namespace DogsServer.Migrations
{
    public partial class ChangedDogTrainingToIncludeDelayTimeAndLostPersonTrackLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DelayTimeTicks",
                table: "DogTrainings",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<double>(
                name: "LostPersonTrackLength",
                table: "DogTrainings",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DelayTimeTicks",
                table: "DogTrainings");

            migrationBuilder.DropColumn(
                name: "LostPersonTrackLength",
                table: "DogTrainings");
        }
    }
}
