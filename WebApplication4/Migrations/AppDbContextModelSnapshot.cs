﻿// <auto-generated />
using System;
using DogsServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DogsServer.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DogsServer.Models.Action", b =>
                {
                    b.Property<int>("ActionID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<int?>("EventID");

                    b.Property<int>("GuideID");

                    b.Property<int>("Level");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Notes");

                    b.Property<int?>("Workmodes");

                    b.HasKey("DogID");

                    b.HasIndex("EventID");

                    b.HasIndex("GuideID");

                    b.ToTable("Dogs");
                });

            modelBuilder.Entity("DogsServer.Models.Employee", b =>
                {
                    b.Property<int>("EmployeeID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("City")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.HasKey("EmployeeID");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("DogsServer.Models.Event", b =>
                {
                    b.Property<int>("EventID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address");

                    b.Property<string>("City");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("IdentityId");

                    b.Property<bool>("IsAdmin");

                    b.Property<bool>("IsMember");

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("Notes");

                    b.Property<string>("Phone");

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
