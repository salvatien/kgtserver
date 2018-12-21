using System;
//using System.Data.Entity;
//using System.Linq;
//using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using DogsServer.Models;
using System.Linq;
using System.Linq.Expressions;

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
            return DbSet.Include(x => x.Comments).Include(x => x.DogTrainings).Where(predicate);
        }

        public new IQueryable<Training> GetAll()
        {
            //return DbSet;
            return DbSet.Include(x => x.Comments).Include(x => x.DogTrainings);
        }

        public Training GetById(int id)
        {
            var training = DbSet.Where(x => x.TrainingId == id).Include(x => x.Comments).Include(x => x.DogTrainings).FirstOrDefault();
            return training;
        }

    }
}
