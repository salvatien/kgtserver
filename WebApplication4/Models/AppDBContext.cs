using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogsServer.Models
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Guide> Guides { get; set; }
        public virtual DbSet<Dog> Dogs { get; set; }
        public virtual DbSet<Action> Actions { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<GuideEvent> GuideEvents { get; set; }
        public virtual DbSet<GuideAction> GuideActions { get; set; }
        protected override void OnConfiguring
        (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = tcp:kgt.database.windows.net, 1433; Initial Catalog = kgtsqldb; Persist Security Info = False; User ID = kgtadmin; Password = Pieskina102; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;");
            //optionsBuilder.UseSqlServer(@"Data Source=WERNER777\SQL777;Initial Catalog=Employees;Integrated Security=SSPI;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GuideAction>().HasKey(ga => new { ga.GuideId, ga.ActionId });
            modelBuilder.Entity<GuideEvent>().HasKey(ge => new { ge.GuideId, ge.EventId });

        }
    }
}
