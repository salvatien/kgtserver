using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DogsServer.Migrations
{
    public partial class AddedLastEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DogAction_Actions_ActionId",
                table: "DogAction");

            migrationBuilder.DropForeignKey(
                name: "FK_DogAction_Dogs_DogId",
                table: "DogAction");

            migrationBuilder.DropForeignKey(
                name: "FK_Dogs_Events_EventId",
                table: "Dogs");

            migrationBuilder.DropIndex(
                name: "IX_Dogs_EventId",
                table: "Dogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DogAction",
                table: "DogAction");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Dogs");

            migrationBuilder.RenameTable(
                name: "DogAction",
                newName: "DogActions");

            migrationBuilder.RenameIndex(
                name: "IX_DogAction_ActionId",
                table: "DogActions",
                newName: "IX_DogActions_ActionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DogActions",
                table: "DogActions",
                columns: new[] { "DogId", "ActionId" });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    CertificateId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Level = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ValidThrough = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.CertificateId);
                });

            migrationBuilder.CreateTable(
                name: "DogEvents",
                columns: table => new
                {
                    DogId = table.Column<int>(nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    LostPerson = table.Column<string>(nullable: true),
                    DogTrackBlobUrl = table.Column<string>(nullable: true),
                    LostPersonTrackBlobUrl = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    Weather = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DogEvents", x => new { x.DogId, x.EventId });
                    table.ForeignKey(
                        name: "FK_DogEvents_Dogs_DogId",
                        column: x => x.DogId,
                        principalTable: "Dogs",
                        principalColumn: "DogId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DogEvents_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DogCertificates",
                columns: table => new
                {
                    DogId = table.Column<int>(nullable: false),
                    CertificateId = table.Column<int>(nullable: false),
                    AcquiredOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DogCertificates", x => new { x.DogId, x.CertificateId });
                    table.ForeignKey(
                        name: "FK_DogCertificates_Certificates_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificates",
                        principalColumn: "CertificateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DogCertificates_Dogs_DogId",
                        column: x => x.DogId,
                        principalTable: "Dogs",
                        principalColumn: "DogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DogCertificates_CertificateId",
                table: "DogCertificates",
                column: "CertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_DogEvents_EventId",
                table: "DogEvents",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_DogActions_Actions_ActionId",
                table: "DogActions",
                column: "ActionId",
                principalTable: "Actions",
                principalColumn: "ActionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DogActions_Dogs_DogId",
                table: "DogActions",
                column: "DogId",
                principalTable: "Dogs",
                principalColumn: "DogId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DogActions_Actions_ActionId",
                table: "DogActions");

            migrationBuilder.DropForeignKey(
                name: "FK_DogActions_Dogs_DogId",
                table: "DogActions");

            migrationBuilder.DropTable(
                name: "DogCertificates");

            migrationBuilder.DropTable(
                name: "DogEvents");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DogActions",
                table: "DogActions");

            migrationBuilder.RenameTable(
                name: "DogActions",
                newName: "DogAction");

            migrationBuilder.RenameIndex(
                name: "IX_DogActions_ActionId",
                table: "DogAction",
                newName: "IX_DogAction_ActionId");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Dogs",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DogAction",
                table: "DogAction",
                columns: new[] { "DogId", "ActionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Dogs_EventId",
                table: "Dogs",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_DogAction_Actions_ActionId",
                table: "DogAction",
                column: "ActionId",
                principalTable: "Actions",
                principalColumn: "ActionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DogAction_Dogs_DogId",
                table: "DogAction",
                column: "DogId",
                principalTable: "Dogs",
                principalColumn: "DogId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dogs_Events_EventId",
                table: "Dogs",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
