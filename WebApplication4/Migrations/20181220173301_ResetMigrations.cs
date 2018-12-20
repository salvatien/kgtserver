using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DogsServer.Migrations
{
    public partial class ResetMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Actions",
            //    columns: table => new
            //    {
            //        ActionId = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Date = table.Column<DateTime>(nullable: false),
            //        City = table.Column<string>(nullable: false),
            //        StreetOrLocation = table.Column<string>(nullable: false),
            //        LostPersonFirstName = table.Column<string>(nullable: false),
            //        LostPersonLastName = table.Column<string>(nullable: false),
            //        Coordinator = table.Column<string>(nullable: false),
            //        WasSuccess = table.Column<bool>(nullable: false),
            //        Notes = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Actions", x => x.ActionId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Events",
            //    columns: table => new
            //    {
            //        EventId = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Title = table.Column<string>(nullable: false),
            //        Date = table.Column<DateTime>(nullable: false),
            //        City = table.Column<string>(nullable: false),
            //        StreetOrLocation = table.Column<string>(nullable: false),
            //        Description = table.Column<string>(nullable: false),
            //        Notes = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Events", x => x.EventId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Guides",
            //    columns: table => new
            //    {
            //        GuideId = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        IdentityId = table.Column<string>(nullable: true),
            //        FirstName = table.Column<string>(nullable: false),
            //        LastName = table.Column<string>(nullable: false),
            //        Email = table.Column<string>(nullable: false),
            //        City = table.Column<string>(nullable: true),
            //        Address = table.Column<string>(nullable: true),
            //        Phone = table.Column<string>(nullable: true),
            //        Notes = table.Column<string>(nullable: true),
            //        IsAdmin = table.Column<bool>(nullable: false),
            //        IsMember = table.Column<bool>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Guides", x => x.GuideId);
            //    });

            migrationBuilder.CreateTable(
                name: "Trainings",
                columns: table => new
                {
                    TrainingId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    GeneralLocation = table.Column<string>(nullable: false),
                    LocationDetails = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainings", x => x.TrainingId);
                });

            //migrationBuilder.CreateTable(
            //    name: "Dogs",
            //    columns: table => new
            //    {
            //        DogId = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Name = table.Column<string>(nullable: false),
            //        Breed = table.Column<string>(nullable: true),
            //        DateOfBirth = table.Column<DateTime>(nullable: false),
            //        Level = table.Column<int>(nullable: false),
            //        Workmodes = table.Column<int>(nullable: true),
            //        PhotoBlobUrl = table.Column<string>(nullable: true),
            //        Notes = table.Column<string>(nullable: true),
            //        GuideId = table.Column<int>(nullable: false),
            //        EventId = table.Column<int>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Dogs", x => x.DogId);
            //        table.ForeignKey(
            //            name: "FK_Dogs_Events_EventId",
            //            column: x => x.EventId,
            //            principalTable: "Events",
            //            principalColumn: "EventId",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Dogs_Guides_GuideId",
            //            column: x => x.GuideId,
            //            principalTable: "Guides",
            //            principalColumn: "GuideId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "GuideActions",
            //    columns: table => new
            //    {
            //        GuideId = table.Column<int>(nullable: false),
            //        ActionId = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_GuideActions", x => new { x.GuideId, x.ActionId });
            //        table.ForeignKey(
            //            name: "FK_GuideActions_Actions_ActionId",
            //            column: x => x.ActionId,
            //            principalTable: "Actions",
            //            principalColumn: "ActionId",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_GuideActions_Guides_GuideId",
            //            column: x => x.GuideId,
            //            principalTable: "Guides",
            //            principalColumn: "GuideId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "GuideEvents",
            //    columns: table => new
            //    {
            //        GuideId = table.Column<int>(nullable: false),
            //        EventId = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_GuideEvents", x => new { x.GuideId, x.EventId });
            //        table.ForeignKey(
            //            name: "FK_GuideEvents_Events_EventId",
            //            column: x => x.EventId,
            //            principalTable: "Events",
            //            principalColumn: "EventId",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_GuideEvents_Guides_GuideId",
            //            column: x => x.GuideId,
            //            principalTable: "Guides",
            //            principalColumn: "GuideId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateTable(
                name: "TrainingComments",
                columns: table => new
                {
                    TrainingCommentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TrainingId = table.Column<int>(nullable: false),
                    AuthorId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Content = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingComments", x => x.TrainingCommentId);
                    table.ForeignKey(
                        name: "FK_TrainingComments_Guides_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Guides",
                        principalColumn: "GuideId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingComments_Trainings_TrainingId",
                        column: x => x.TrainingId,
                        principalTable: "Trainings",
                        principalColumn: "TrainingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DogAction",
                columns: table => new
                {
                    DogId = table.Column<int>(nullable: false),
                    ActionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DogAction", x => new { x.DogId, x.ActionId });
                    table.ForeignKey(
                        name: "FK_DogAction_Actions_ActionId",
                        column: x => x.ActionId,
                        principalTable: "Actions",
                        principalColumn: "ActionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DogAction_Dogs_DogId",
                        column: x => x.DogId,
                        principalTable: "Dogs",
                        principalColumn: "DogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DogTrainings",
                columns: table => new
                {
                    TrainingId = table.Column<int>(nullable: false),
                    DogId = table.Column<int>(nullable: false),
                    LostPerson = table.Column<string>(nullable: true),
                    DogTrackBlobUrl = table.Column<string>(nullable: true),
                    LostPersonTrackBlobUrl = table.Column<string>(nullable: true),
                    Weather = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DogTrainings", x => new { x.DogId, x.TrainingId });
                    table.ForeignKey(
                        name: "FK_DogTrainings_Dogs_DogId",
                        column: x => x.DogId,
                        principalTable: "Dogs",
                        principalColumn: "DogId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DogTrainings_Trainings_TrainingId",
                        column: x => x.TrainingId,
                        principalTable: "Trainings",
                        principalColumn: "TrainingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DogTrainingComments",
                columns: table => new
                {
                    DogTrainingCommentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TrainingId = table.Column<int>(nullable: false),
                    DogId = table.Column<int>(nullable: false),
                    AuthorId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Content = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DogTrainingComments", x => x.DogTrainingCommentId);
                    table.ForeignKey(
                        name: "FK_DogTrainingComments_Guides_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Guides",
                        principalColumn: "GuideId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DogTrainingComments_DogTrainings_DogId_TrainingId",
                        columns: x => new { x.DogId, x.TrainingId },
                        principalTable: "DogTrainings",
                        principalColumns: new[] { "DogId", "TrainingId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DogAction_ActionId",
                table: "DogAction",
                column: "ActionId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Dogs_EventId",
            //    table: "Dogs",
            //    column: "EventId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Dogs_GuideId",
            //    table: "Dogs",
            //    column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_DogTrainingComments_AuthorId",
                table: "DogTrainingComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_DogTrainingComments_DogId_TrainingId",
                table: "DogTrainingComments",
                columns: new[] { "DogId", "TrainingId" });

            migrationBuilder.CreateIndex(
                name: "IX_DogTrainings_TrainingId",
                table: "DogTrainings",
                column: "TrainingId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_GuideActions_ActionId",
            //    table: "GuideActions",
            //    column: "ActionId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_GuideEvents_EventId",
            //    table: "GuideEvents",
            //    column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingComments_AuthorId",
                table: "TrainingComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingComments_TrainingId",
                table: "TrainingComments",
                column: "TrainingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DogAction");

            migrationBuilder.DropTable(
                name: "DogTrainingComments");

            migrationBuilder.DropTable(
                name: "GuideActions");

            migrationBuilder.DropTable(
                name: "GuideEvents");

            migrationBuilder.DropTable(
                name: "TrainingComments");

            migrationBuilder.DropTable(
                name: "DogTrainings");

            migrationBuilder.DropTable(
                name: "Actions");

            migrationBuilder.DropTable(
                name: "Dogs");

            migrationBuilder.DropTable(
                name: "Trainings");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Guides");
        }
    }
}
