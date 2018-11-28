using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DogsServer.Migrations
{
    public partial class ReplacedDogWorkmodeWrapperWithFlagsEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DogWorkmodeWrapper");

            migrationBuilder.AddColumn<int>(
                name: "Workmodes",
                table: "Dogs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Workmodes",
                table: "Dogs");

            migrationBuilder.CreateTable(
                name: "DogWorkmodeWrapper",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DogID = table.Column<int>(nullable: true),
                    Workmode = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DogWorkmodeWrapper", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DogWorkmodeWrapper_Dogs_DogID",
                        column: x => x.DogID,
                        principalTable: "Dogs",
                        principalColumn: "DogID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DogWorkmodeWrapper_DogID",
                table: "DogWorkmodeWrapper",
                column: "DogID");
        }
    }
}
