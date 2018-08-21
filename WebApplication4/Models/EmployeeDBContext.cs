using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication4.Models
{
    public class EmployeeDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring
        (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = tcp:kgt.database.windows.net, 1433; Initial Catalog = kgtsqldb; Persist Security Info = False; User ID = kgtadmin; Password = Pieskina102; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;");
            //optionsBuilder.UseSqlServer(@"Data Source=WERNER777\SQL777;Initial Catalog=Employees;Integrated Security=SSPI;");
        }
    }
}
