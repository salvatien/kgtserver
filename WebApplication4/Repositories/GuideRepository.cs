﻿using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using Dogs.Data.Models;

namespace DogsServer.Repositories
{
    public class GuideRepository : Repository<Guide>
    {
        protected new DbSet<Guide> DbSet;

        public GuideRepository(DbContext dataContext)
             : base(dataContext)
        {
            DbSet = dataContext.Set<Guide>();
        }


        public new IQueryable<Guide> SearchFor(Expression<Func<Guide, bool>> predicate)
        {
            return DbSet.Include(x=> x.Dogs).Include(x => x.GuideEvents).Where(predicate);
        }

        public new IQueryable<Guide> GetAll()
        {
            return DbSet.Include(x => x.Dogs).Include(x => x.GuideEvents);
        }

        public Guide GetById(int id)
        {
            //return DbSet.Find(id);
            //var dog = DbSet.Where(x => x.DogID == id).Include(x => x.Guide).FirstOrDefault();
            //return dog;
            var guide = DbSet.Where(x => x.GuideId == id).Include(x => x.Dogs).Include(x => x.GuideEvents).FirstOrDefault();
            return guide;
        }

        public int GetFreeId()
        {
            var id = DbSet.OrderByDescending(u => u.GuideId).FirstOrDefault().GuideId + 1;
            return id;
        }

    }
}
