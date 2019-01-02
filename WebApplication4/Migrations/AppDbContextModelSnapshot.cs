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

            modelBuilder.Entity("DogsServer.Models.Certificate", b =>
                {
                    b.Property<int>("CertificateId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("Level");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<long>("ValidThroughTicks");

                    b.HasKey("CertificateId");

                    b.ToTable("Certificates");
                });

            modelBuilder.Entity("DogsServer.Models.Dog", b =>
                {
                    b.Property<int>("DogId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Breed");

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<int>("GuideId");

                    b.Property<int>("Level");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Notes");

                    b.Property<string>("PhotoBlobUrl");

                    b.Property<int?>("Workmodes");

                    b.HasKey("DogId");

                    b.HasIndex("GuideId");

                    b.ToTable("Dogs");
                });

            modelBuilder.Entity("DogsServer.Models.DogCertificate", b =>
                {
                    b.Property<int>("DogId");

                    b.Property<int>("CertificateId");

                    b.Property<DateTime>("AcquiredOn");

                    b.HasKey("DogId", "CertificateId");

                    b.HasIndex("CertificateId");

                    b.ToTable("DogCertificates");
                });

            modelBuilder.Entity("DogsServer.Models.DogEvent", b =>
                {
                    b.Property<int>("DogId");

                    b.Property<int>("EventId");

                    b.Property<string>("DogTrackBlobUrl");

                    b.Property<string>("LostPerson");

                    b.Property<string>("LostPersonTrackBlobUrl");

                    b.Property<string>("Notes");

                    b.Property<string>("Weather");

                    b.HasKey("DogId", "EventId");

                    b.HasIndex("EventId");

                    b.ToTable("DogEvents");
                });

            modelBuilder.Entity("DogsServer.Models.DogTraining", b =>
                {
                    b.Property<int>("DogId");

                    b.Property<int>("TrainingId");

                    b.Property<string>("DogTrackBlobUrl");

                    b.Property<string>("LostPerson");

                    b.Property<string>("LostPersonTrackBlobUrl");

                    b.Property<string>("Notes");

                    b.Property<string>("Weather");

                    b.HasKey("DogId", "TrainingId");

                    b.HasIndex("TrainingId");

                    b.ToTable("DogTrainings");
                });

            modelBuilder.Entity("DogsServer.Models.DogTrainingComment", b =>
                {
                    b.Property<int>("DogTrainingCommentId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AuthorId");

                    b.Property<string>("Content")
                        .IsRequired();

                    b.Property<DateTime>("Date");

                    b.Property<int>("DogId");

                    b.Property<int>("TrainingId");

                    b.HasKey("DogTrainingCommentId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("DogId", "TrainingId");

                    b.ToTable("DogTrainingComments");
                });

            modelBuilder.Entity("DogsServer.Models.Event", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("EventId")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("City")
                        .IsRequired();

                    b.Property<DateTime>("Date");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<bool>("IsCommercialTraining");

                    b.Property<string>("Notes");

                    b.Property<string>("StreetOrLocation")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("EventId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("DogsServer.Models.Guide", b =>
                {
                    b.Property<int>("GuideId")
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

                    b.HasKey("GuideId");

                    b.ToTable("Guides");
                });

            modelBuilder.Entity("DogsServer.Models.GuideEvent", b =>
                {
                    b.Property<int>("GuideId");

                    b.Property<int>("EventId");

                    b.HasKey("GuideId", "EventId");

                    b.HasIndex("EventId");

                    b.ToTable("GuideEvents");
                });

            modelBuilder.Entity("DogsServer.Models.Training", b =>
                {
                    b.Property<int>("TrainingId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<string>("GeneralLocation")
                        .IsRequired();

                    b.Property<string>("LocationDetails");

                    b.Property<string>("Notes");

                    b.HasKey("TrainingId");

                    b.ToTable("Trainings");
                });

            modelBuilder.Entity("DogsServer.Models.TrainingComment", b =>
                {
                    b.Property<int>("TrainingCommentId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AuthorId");

                    b.Property<string>("Content")
                        .IsRequired();

                    b.Property<DateTime>("Date");

                    b.Property<int>("TrainingId");

                    b.HasKey("TrainingCommentId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("TrainingId");

                    b.ToTable("TrainingComments");
                });

            modelBuilder.Entity("DogsServer.Models.Dog", b =>
                {
                    b.HasOne("DogsServer.Models.Guide", "Guide")
                        .WithMany("Dogs")
                        .HasForeignKey("GuideId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DogsServer.Models.DogCertificate", b =>
                {
                    b.HasOne("DogsServer.Models.Certificate", "Certificate")
                        .WithMany("DogCertificates")
                        .HasForeignKey("CertificateId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DogsServer.Models.Dog", "Dog")
                        .WithMany("DogCertificates")
                        .HasForeignKey("DogId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DogsServer.Models.DogEvent", b =>
                {
                    b.HasOne("DogsServer.Models.Dog", "Dog")
                        .WithMany("DogEvents")
                        .HasForeignKey("DogId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DogsServer.Models.Event", "Event")
                        .WithMany("DogEvents")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DogsServer.Models.DogTraining", b =>
                {
                    b.HasOne("DogsServer.Models.Dog", "Dog")
                        .WithMany("DogTrainings")
                        .HasForeignKey("DogId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DogsServer.Models.Training", "Training")
                        .WithMany("DogTrainings")
                        .HasForeignKey("TrainingId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DogsServer.Models.DogTrainingComment", b =>
                {
                    b.HasOne("DogsServer.Models.Guide", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DogsServer.Models.DogTraining", "DogTraining")
                        .WithMany("Comments")
                        .HasForeignKey("DogId", "TrainingId")
                        .OnDelete(DeleteBehavior.Restrict);
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

            modelBuilder.Entity("DogsServer.Models.TrainingComment", b =>
                {
                    b.HasOne("DogsServer.Models.Guide", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DogsServer.Models.Training", "Training")
                        .WithMany("Comments")
                        .HasForeignKey("TrainingId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
