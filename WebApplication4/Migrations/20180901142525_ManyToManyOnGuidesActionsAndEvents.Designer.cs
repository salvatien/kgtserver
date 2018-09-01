﻿// <auto-generated />
using System;
using DogsServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DogsServer.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20180901142525_ManyToManyOnGuidesActionsAndEvents")]
    partial class ManyToManyOnGuidesActionsAndEvents
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.2-rtm-30932")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DogsServer.Models.Action", b =>
                {
                    b.Property<int>("ActionID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City")
                        .IsRequired();

                    b.Property<string>("Coordinator")
                        .IsRequired();

                    b.Property<DateTime>("Date");

                    b.Property<string>("LostPersonFirstName")
                        .IsRequired();

                    b.Property<string>("LostPersonLastName")
                        .IsRequired();

                    b.Property<string>("Notes");

                    b.Property<string>("StreetOrLocation")
                        .IsRequired();

                    b.Property<bool>("WasSuccess");

                    b.HasKey("ActionID");

                    b.ToTable("Actions");
                });

            modelBuilder.Entity("DogsServer.Models.Dog", b =>
                {
                    b.Property<int>("DogID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<int?>("EventID");

                    b.Property<int>("GuideID");

                    b.Property<int>("Level");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Notes");

                    b.HasKey("DogID");

                    b.HasIndex("EventID");

                    b.HasIndex("GuideID");

                    b.ToTable("Dogs");
                });

            modelBuilder.Entity("DogsServer.Models.Employee", b =>
                {
                    b.Property<int>("EmployeeID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.HasKey("EmployeeID");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("DogsServer.Models.Enums.Wrappers.DogWorkmodeWrapper", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("DogID");

                    b.Property<int>("Workmode");

                    b.HasKey("Id");

                    b.HasIndex("DogID");

                    b.ToTable("DogWorkmodeWrapper");
                });

            modelBuilder.Entity("DogsServer.Models.Event", b =>
                {
                    b.Property<int>("EventID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City")
                        .IsRequired();

                    b.Property<DateTime>("Date");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("Notes");

                    b.Property<string>("StreetOrLocation")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("EventID");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("DogsServer.Models.Guide", b =>
                {
                    b.Property<int>("GuideID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<string>("City")
                        .IsRequired();

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("Fitness");

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("Notes");

                    b.Property<string>("Phone")
                        .IsRequired();

                    b.HasKey("GuideID");

                    b.ToTable("Guides");
                });

            modelBuilder.Entity("DogsServer.Models.GuideAction", b =>
                {
                    b.Property<int>("GuideId");

                    b.Property<int>("ActionId");

                    b.HasKey("GuideId", "ActionId");

                    b.HasIndex("ActionId");

                    b.ToTable("GuideActions");
                });

            modelBuilder.Entity("DogsServer.Models.GuideEvent", b =>
                {
                    b.Property<int>("GuideId");

                    b.Property<int>("EventId");

                    b.HasKey("GuideId", "EventId");

                    b.HasIndex("EventId");

                    b.ToTable("GuideEvents");
                });

            modelBuilder.Entity("DogsServer.Models.Dog", b =>
                {
                    b.HasOne("DogsServer.Models.Event")
                        .WithMany("Dogs")
                        .HasForeignKey("EventID");

                    b.HasOne("DogsServer.Models.Guide", "Guide")
                        .WithMany("Dogs")
                        .HasForeignKey("GuideID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DogsServer.Models.Enums.Wrappers.DogWorkmodeWrapper", b =>
                {
                    b.HasOne("DogsServer.Models.Dog")
                        .WithMany("Workmode")
                        .HasForeignKey("DogID");
                });

            modelBuilder.Entity("DogsServer.Models.GuideAction", b =>
                {
                    b.HasOne("DogsServer.Models.Action", "Action")
                        .WithMany("GuideActions")
                        .HasForeignKey("ActionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DogsServer.Models.Guide", "Guide")
                        .WithMany("GuideActions")
                        .HasForeignKey("GuideId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DogsServer.Models.GuideEvent", b =>
                {
                    b.HasOne("DogsServer.Models.Event", "Event")
                        .WithMany("GuideEvents")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DogsServer.Models.Guide", "Guide")
                        .WithMany("GuideEvents")
                        .HasForeignKey("GuideId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
