using Microsoft.EntityFrameworkCore.Migrations;

namespace DogsServer.Migrations
{
    public partial class ManyToManyOnGuidesActionsAndEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guides_Actions_ActionID",
                table: "Guides");

            migrationBuilder.DropForeignKey(
                name: "FK_Guides_Events_EventID",
                table: "Guides");

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
                        principalColumn: "ActionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuideActions_Guides_GuideId",
                        column: x => x.GuideId,
                        principalTable: "Guides",
                        principalColumn: "GuideID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuideEvents",
                columns: table => new
                {
                    GuideId = table.Column<int>(nullable: false),
                    EventId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideEvents", x => new { x.GuideId, x.EventId });
                    table.ForeignKey(
                        name: "FK_GuideEvents_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuideEvents_Guides_GuideId",
                        column: x => x.GuideId,
                        principalTable: "Guides",
                        principalColumn: "GuideID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuideActions_ActionId",
                table: "GuideActions",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideEvents_EventId",
                table: "GuideEvents",
                column: "EventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuideActions");

            migrationBuilder.DropTable(
                name: "GuideEvents");

            migrationBuilder.AddColumn<int>(
                name: "ActionID",
                table: "Guides",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventID",
                table: "Guides",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guides_ActionID",
                table: "Guides",
                column: "ActionID");

            migrationBuilder.CreateIndex(
                name: "IX_Guides_EventID",
                table: "Guides",
                column: "EventID");

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
    }
}
