using Microsoft.EntityFrameworkCore;
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
        public virtual DbSet<Action> Actions { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<GuideEvent> GuideEvents { get; set; }
        public virtual DbSet<GuideAction> GuideActions { get; set; }
        public virtual DbSet<Training> Trainings { get; set; }
        public virtual DbSet<DogTraining> DogTrainings { get; set; }
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
            modelBuilder.Entity<GuideAction>().HasKey(ga => new { ga.GuideId, ga.ActionId });
            modelBuilder.Entity<DogAction>().HasKey(da => new { da.DogId, da.ActionId });
            modelBuilder.Entity<GuideEvent>().HasKey(ge => new { ge.GuideId, ge.EventId });
            modelBuilder.Entity<DogTraining>().HasKey(dt => new { dt.DogId, dt.TrainingId });

            modelBuilder.Entity<TrainingComment>().HasKey(m => m.TrainingCommentId);
            modelBuilder.Entity<TrainingComment>().HasOne(m => m.Training)
                .WithMany(m => m.Comments)
                .HasForeignKey(m => m.TrainingId)
                .OnDelete(DeleteBehavior.Restrict); // added OnDelete to avoid sercular reference

            modelBuilder.Entity<DogTrainingComment>().HasKey(m => m.DogTrainingCommentId);
            modelBuilder.Entity<DogTrainingComment>().HasOne(m => m.DogTraining)
                .WithMany(m => m.Comments)
                .HasForeignKey(m => new { m.DogId, m.TrainingId })
                .OnDelete(DeleteBehavior.Restrict); // added OnDelete to avoid sercular reference

            modelBuilder.Entity<GuideAction>().HasOne(m => m.Action)
                .WithMany(m => m.GuideActions)
                .HasForeignKey(m => m.ActionId);

            modelBuilder.Entity<GuideEvent>().HasOne(m => m.Event)
                .WithMany(m => m.GuideEvents)
                .HasForeignKey(m => m.EventId);

            modelBuilder.Entity<Training>().HasMany(t => t.DogTrainings)
                .WithOne(dt => dt.Training)
                .HasForeignKey(dt => dt.TrainingId);

            modelBuilder.Entity<Dog>().HasMany(d => d.DogTrainings)
                .WithOne(dt => dt.Dog)
                .HasForeignKey(dt => dt.DogId);
        }
    }
}
