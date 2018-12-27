using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DogsServer.Migrations
{
    public partial class RemoveActionRelatedEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DogActions");

            migrationBuilder.DropTable(
                name: "GuideActions");

            migrationBuilder.DropTable(
                name: "Actions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actions",
                columns: table => new
                {
                    ActionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    City = table.Column<string>(nullable: false),
                    Coordinator = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    LostPersonFirstName = table.Column<string>(nullable: false),
                    LostPersonLastName = table.Column<string>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    StreetOrLocation = table.Column<string>(nullable: false),
                    WasSuccess = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.ActionId);
                });

            migrationBuilder.CreateTable(
                name: "DogActions",
                columns: table => new
                {
                    DogId = table.Column<int>(nullable: false),
                    ActionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DogActions", x => new { x.DogId, x.ActionId });
                    table.ForeignKey(
                        name: "FK_DogActions_Actions_ActionId",
                        column: x => x.ActionId,
                        principalTable: "Actions",
                        principalColumn: "ActionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DogActions_Dogs_DogId",
                        column: x => x.DogId,
                        principalTable: "Dogs",
                        principalColumn: "DogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuideActions",
                columns: table => new
                {
                    GuideId = table.Column<int>(nullable: false),
                    ActionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideActions", x => new { x.GuideId, x.ActionId });
                    table.ForeignKey(
                        name: "FK_GuideActions_Actions_ActionId",
                        column: x => x.ActionId,
                        principalTable: "Actions",
                        principalColumn: "ActionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuideActions_Guides_GuideId",
                        column: x => x.GuideId,
                        principalTable: "Guides",
                        principalColumn: "GuideId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DogActions_ActionId",
                table: "DogActions",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideActions_ActionId",
                table: "GuideActions",
                column: "ActionId");
        }
    }
}
