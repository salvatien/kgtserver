using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using Dogs.Data.Models;


namespace DogsServer.Repositories
{
    public class CertificateRepository : Repository<Certificate>
    {
        protected new DbSet<Certificate> DbSet;

        public CertificateRepository(DbContext dataContext)
             : base(dataContext)
        {
            DbSet = dataContext.Set<Certificate>();
        }


        public new IQueryable<Certificate> SearchFor(Expression<Func<Certificate, bool>> predicate)
        {
            return DbSet.Include(x => x.DogCertificates).Where(predicate);
        }

        public new IQueryable<Certificate> GetAll()
        {
            return DbSet.Include(x => x.DogCertificates);
        }

        public Certificate GetById(int id)
        {
            //return DbSet.Find(id);
            //var dog = DbSet.Where(x => x.DogID == id).Include(x => x.Guide).FirstOrDefault();
            //return dog;
            var cert = DbSet.Where(x => x.CertificateId == id).Include(x => x.DogCertificates).FirstOrDefault();
            return cert;
        }
    }
}