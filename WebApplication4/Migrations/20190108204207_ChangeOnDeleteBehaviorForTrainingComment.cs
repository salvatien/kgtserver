using Microsoft.EntityFrameworkCore.Migrations;

namespace DogsServer.Migrations
{
    public partial class ChangeOnDeleteBehaviorForTrainingComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingComments_Trainings_TrainingId",
                table: "TrainingComments");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingComments_Trainings_TrainingId",
                table: "TrainingComments",
                column: "TrainingId",
                principalTable: "Trainings",
                principalColumn: "TrainingId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingComments_Trainings_TrainingId",
                table: "TrainingComments");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingComments_Trainings_TrainingId",
                table: "TrainingComments",
                column: "TrainingId",
                principalTable: "Trainings",
                principalColumn: "TrainingId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
