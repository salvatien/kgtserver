﻿using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using Dogs.Data.Models;

namespace DogsServer.Repositories
{
    public class TrainingRepository : Repository<Training>
    {
        protected new DbSet<Training> DbSet;

        public TrainingRepository(DbContext dataContext)
             : base(dataContext)
        {
            DbSet = dataContext.Set<Training>();
        }


        public new IQueryable<Training> SearchFor(Expression<Func<Training, bool>> predicate)
        {
            return DbSet.Include(x => x.Comments).ThenInclude(c => c.Author)
                .Include(x => x.DogTrainings).ThenInclude(dt => dt.Dog).ThenInclude(d => d.Guide).Where(predicate);
        }

        public new IQueryable<Training> GetAll()
        {
            //return DbSet;
            return DbSet.Include(x => x.Comments).ThenInclude(c => c.Author)
                .Include(x => x.DogTrainings).ThenInclude( dt => dt.Dog).ThenInclude(d => d.Guide);
        }

        public Training GetById(int id)
        {
            var training = DbSet.Where(x => x.TrainingId == id).Include(x => x.Comments).ThenInclude(c => c.Author)
                .Include(x => x.DogTrainings).ThenInclude(dt => dt.Dog).ThenInclude(d => d.Guide).FirstOrDefault();
            return training;
        }

    }
}
