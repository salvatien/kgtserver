﻿using System;
//using System.Data.Entity;
//using System.Linq;
//using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using DogsServer.Models;
using System.Linq;
using System.Linq.Expressions;

namespace DogsServer.Repositories
{
    public class DogCertificateRepository : Repository<DogCertificate>
    {
        protected new DbSet<DogCertificate> DbSet;

        public DogCertificateRepository(DbContext dataContext)
             : base(dataContext)
        {
            DbSet = dataContext.Set<DogCertificate>();
        }


        public new IQueryable<DogCertificate> SearchFor(Expression<Func<DogCertificate, bool>> predicate)
        {
            return DbSet.Include(x => x.Dog).Include(x => x.Certificate).Where(predicate);
        }

        public new IQueryable<DogCertificate> GetAll()
        {
            //return DbSet;
            return DbSet.Include(x => x.Dog).Include(x => x.Certificate);
        }

        public DogCertificate GetByIds(int dogId, int certificateId)
        {
            var DogCertificate = DbSet.Where(x => x.CertificateId == certificateId && x.DogId == dogId).Include(x => x.Dog).Include(x => x.Certificate).FirstOrDefault();
            return DogCertificate;
        }
    }
}