using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DogsServer.Migrations
{
    public partial class AddedDogsActionsAndEventsModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActionID",
                table: "Guides",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventID",
                table: "Guides",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Actions",
                columns: table => new
                {
                    ActionID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    City = table.Column<string>(nullable: false),
                    StreetOrLocation = table.Column<string>(nullable: false),
                    LostPersonFirstName = table.Column<string>(nullable: false),
                    LostPersonLastName = table.Column<string>(nullable: false),
                    Coordinator = table.Column<string>(nullable: false),
                    WasSuccess = table.Column<bool>(nullable: false),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.ActionID);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    City = table.Column<string>(nullable: false),
                    StreetOrLocation = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventID);
                });

            migrationBuilder.CreateTable(
                name: "Dogs",
                columns: table => new
                {
                    DogID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    GuideID = table.Column<int>(nullable: false),
                    EventID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dogs", x => x.DogID);
                    table.ForeignKey(
                        name: "FK_Dogs_Events_EventID",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "EventID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dogs_Guides_GuideID",
                        column: x => x.GuideID,
                        principalTable: "Guides",
                        principalColumn: "GuideID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DogWorkmodeWrapper",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Workmode = table.Column<int>(nullable: false),
                    DogID = table.Column<int>(nullable: true)
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
                name: "IX_Guides_ActionID",
                table: "Guides",
                column: "ActionID");

            migrationBuilder.CreateIndex(
                name: "IX_Guides_EventID",
                table: "Guides",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_Dogs_EventID",
                table: "Dogs",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_Dogs_GuideID",
                table: "Dogs",
                column: "GuideID");

            migrationBuilder.CreateIndex(
                name: "IX_DogWorkmodeWrapper_DogID",
                table: "DogWorkmodeWrapper",
                column: "DogID");

            migrationBuilder.AddForeignKey(
                name: "FK_Guides_Actions_ActionID",
                table: "Guides",
                column: "ActionID",
                principalTable: "Actions",
                principalColumn: "ActionID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Guides_Events_EventID",
                table: "Guides",
                column: "EventID",
                principalTable: "Events",
                principalColumn: "EventID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guides_Actions_ActionID",
                table: "Guides");

            migrationBuilder.DropForeignKey(
                name: "FK_Guides_Events_EventID",
                table: "Guides");

            migrationBuilder.DropTable(
                name: "Actions");

            migrationBuilder.DropTable(
                name: "DogWorkmodeWrapper");

            migrationBuilder.DropTable(
                name: "Dogs");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Guides_ActionID",
                table: "Guides");

            migrationBuilder.DropIndex(
                name: "IX_Guides_EventID",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "ActionID",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "EventID",
                table: "Guides");
        }
    }
}
