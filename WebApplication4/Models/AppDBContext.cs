﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogsServer.Models
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<Guide> Guides { get; set; }
        public virtual DbSet<Dog> Dogs { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Certificate> Certificates { get; set; }
        public virtual DbSet<DogCertificate> DogCertificates { get; set; }
        public virtual DbSet<GuideEvent> GuideEvents { get; set; }
        public virtual DbSet<Training> Trainings { get; set; }
        public virtual DbSet<DogTraining> DogTrainings { get; set; }
        public virtual DbSet<DogEvent> DogEvents { get; set; }
        public virtual DbSet<TrainingComment> TrainingComments { get; set; }
        public virtual DbSet<DogTrainingComment> DogTrainingComments { get; set; }
        protected override void OnConfiguring
        (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = tcp:kgt.database.windows.net, 1433; Initial Catalog = kgtsqldb; Persist Security Info = False; User ID = kgtadmin; Password = Pieskina102; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;");
            //optionsBuilder.UseSqlServer(@"Data Source=WERNER777\SQL777;Initial Catalog=Employees;Integrated Security=SSPI;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GuideEvent>().HasKey(ge => new { ge.GuideId, ge.EventId });
            modelBuilder.Entity<DogTraining>().HasKey(dt => new { dt.DogId, dt.TrainingId });
            modelBuilder.Entity<DogEvent>().HasKey(de => new { de.DogId, de.EventId });
            modelBuilder.Entity<DogCertificate>().HasKey(dc => new { dc.DogId, dc.CertificateId });

            modelBuilder.Entity<TrainingComment>().HasKey(m => m.TrainingCommentId);
            modelBuilder.Entity<DogTrainingComment>().HasKey(m => m.DogTrainingCommentId);

            modelBuilder.Entity<TrainingComment>().HasOne(m => m.Training)
                .WithMany(m => m.Comments)
                .HasForeignKey(m => m.TrainingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DogTrainingComment>().HasOne(m => m.DogTraining)
                .WithMany(m => m.Comments)
                .HasForeignKey(m => new { m.DogId, m.TrainingId })
                .OnDelete(DeleteBehavior.Restrict); // added OnDelete to avoid sercular reference

            //modelBuilder.Entity<GuideAction>().HasOne(m => m.Action)
            //    .WithMany(m => m.GuideActions)
            //    .HasForeignKey(m => m.ActionId);

            //modelBuilder.Entity<GuideEvent>().HasOne(m => m.Event)
            //    .WithMany(m => m.GuideEvents)
            //    .HasForeignKey(m => m.EventId);


            //dog trainings
            modelBuilder.Entity<Training>().HasMany(t => t.DogTrainings)
                .WithOne(dt => dt.Training)
                .HasForeignKey(dt => dt.TrainingId);

            modelBuilder.Entity<Dog>().HasMany(d => d.DogTrainings)
                .WithOne(dt => dt.Dog)
                .HasForeignKey(dt => dt.DogId);


            //dog events
            modelBuilder.Entity<Event>().HasMany(t => t.DogEvents)
                .WithOne(de => de.Event)
                .HasForeignKey(de => de.EventId);

            modelBuilder.Entity<Dog>().HasMany(d => d.DogEvents)
                .WithOne(de => de.Dog)
                .HasForeignKey(de => de.DogId);

            //guide events
            modelBuilder.Entity<Guide>().HasMany(g => g.GuideEvents)
                .WithOne(ge => ge.Guide)
                .HasForeignKey(ge => ge.GuideId);

            modelBuilder.Entity<Event>().HasMany(e => e.GuideEvents)
                .WithOne(ge => ge.Event)
                .HasForeignKey(ge => ge.EventId);


            //dog certificates
            modelBuilder.Entity<Dog>().HasMany(d => d.DogCertificates)
                .WithOne(dc => dc.Dog)
                .HasForeignKey(dc => dc.DogId);

            modelBuilder.Entity<Certificate>().HasMany(c => c.DogCertificates)
                .WithOne(dc => dc.Certificate)
                .HasForeignKey(dc => dc.CertificateId);
        }
    }
}
